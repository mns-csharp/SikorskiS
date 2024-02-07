using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace Figure_7_Sikorski
{
    public class MultipleMonteCarloProcessor
    {
        private string pattern_;
        private readonly string rootDirectory_;
        public List<double> XList { get; private set; }
        public List<double> YList { get; private set; }
        public ListPairs ReferenceLines { get; private set; } = new ListPairs();

        public MultipleMonteCarloProcessor(string rootDirectory, string pattern)
        {
            rootDirectory_ = rootDirectory;
            pattern_ = pattern;
        }

        public void ProcessData(string outputPlotPath, int numLags, int tau, bool normAutocorrFlag)
        {
            try
            {
                DirectoryFilter filter = new DirectoryFilter(rootDirectory_, pattern_);
                List<string> mc0123directories = filter.GetMatchingDirectories();
                ListPairs multipleMCfolders = new ListPairs();
                foreach (var mcDir in mc0123directories)
                {
                    Console.WriteLine($"Now processing simulation {mcDir}");
                    SingleMonteCarloFolderProcessor simProcessor = new SingleMonteCarloFolderProcessor(mcDir);
                    simProcessor.ProcessData(outputPlotPath, numLags, tau, normAutocorrFlag);

                    List<double> residueLen = simProcessor.ResidueLengths;
                    List<double> relaxation = simProcessor.RouseRelaxation;

                    multipleMCfolders.Add(simProcessor.SimulationID, residueLen, relaxation);
                }

                List<double> concatResidueLenList = multipleMCfolders.GetComputeConcatenatedUniqueXList();
                List<double> contatRelaxationList = multipleMCfolders.GetComputeMeanY();

                Tuple<List<double>, List<double>> rouseTuple = ListUtils.Sort(concatResidueLenList, contatRelaxationList);
                XList = rouseTuple.Item1;
                YList = rouseTuple.Item2;

                FileWriter.WriteToFile(outputPlotPath, "Fig7PlotPoints.txt", XList, YList);

                //Tuple<List<double>, List<double>> regressionTuple = FitInLogScale.CreateRegressionLine(XList, YList); 
                //ReferenceLines.Add("RegressionLine", regressionTuple.Item1, regressionTuple.Item2);

                Tuple<List<double>, List<double>> slope2Tuple = FitInLogScale.GetNewLineOfSlopeIntersectingGivenLine(2.0, XList, YList);
                ReferenceLines.Add("Slope2Line", slope2Tuple.Item1, slope2Tuple.Item2);
            }
            catch (Exception ex)
            {
                // Handle the exception, e.g. log it or throw it.
                // Never swallow exceptions without handling them properly.
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

