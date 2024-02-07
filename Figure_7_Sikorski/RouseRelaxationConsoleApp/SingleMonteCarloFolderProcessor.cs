using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Figure_7_Sikorski
{ 
    public class SingleMonteCarloFolderProcessor
    {   
        public string SimulationID { get; private set; }
        public string SimulationDirectoryPath { get; private set; }
        public List<string> RunDirectories { get; private set; }
        public List<double> ResidueLengths { get; private set; } = new List<double>();
        public List<double> RouseRelaxation { get; private set; } = new List<double>();

        public SingleMonteCarloFolderProcessor(string simulationDirectoryPath)
        {
            SimulationDirectoryPath = simulationDirectoryPath;
            SimulationID = GetSimulationDirName(SimulationDirectoryPath);
            DirectoryFilter filter = new DirectoryFilter(SimulationDirectoryPath, Constants.SingleRunDirPattern);
            RunDirectories = filter.GetMatchingDirectories();
        }

        public void ProcessData(string outputPlotPath, int numLags, int tau, bool normAutocorrFlag)
        {
            ResidueLengths = new List<double>();
            RouseRelaxation = new List<double>();
            RvaluesProcessor processor = new RvaluesProcessor(SimulationID);
            foreach (var dirPath in RunDirectories)
            {
                Console.WriteLine($"Now processing residue: {dirPath}");
                try
                {
                    processor.ProcessData(dirPath, outputPlotPath, numLags, tau, normAutocorrFlag);                    
                    RouseRelaxation.Add(processor.Tau0Value);
                    ResidueLengths.Add(processor.ResidueLength);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"r2.dat file not found in directory: {dirPath}");
                }
                catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        #region MyRegion
        private string GetSimulationDirName(string path)
        {
            return Path.GetFileName(path);
        } 
        #endregion
    }
}
