using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Figure_7_Sikorski
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string rootPath = Settings.InputDirectory;
                string solutionPath = string.Empty;

                if (Settings.DataFileType == DataFileTypeEnum.R2)
                {
                    solutionPath = Path.Combine(Settings.OutputDirectory, "[r2.dat]");
                }
                else if (Settings.DataFileType == DataFileTypeEnum.RendVec)
                {
                    solutionPath = Path.Combine(Settings.OutputDirectory, "[r_end_vec.dat]");
                }
            
                //Output directory created
                string dirDateTime = DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
                string outputPlotPath = Path.Combine(solutionPath, $"Figure_7_{dirDateTime}");

                // Create a new graph
                var plotter = new DataPlotter();
                plotter.ImageTitle = "Figure-7: chain length vs. longest relaxation time plot";
                plotter.XAxisTitle = "N (chain length)";
                plotter.YAxisTitle = "τR (longest relaxation time)";

                plotter.IsLogY = true;
                plotter.IsLogX = true;
                //plotter.SetXAxisScaleLog(1, 5);
                plotter.IsSymbolVisible = true;
                plotter.IsLegendVisible = true;

                //string strPattern = @"^mc\d{3}$";

                //const int numLags = 1000;
                int numLags = Settings.NumAutoCorrLags;
                bool normAutocorrFlag = Settings.NormAutoCorr;

                const int tau = 1;


                //Process data
                MultipleMonteCarloProcessor processor = new MultipleMonteCarloProcessor(rootPath, Constants.MultipleRunPattern);
                processor.ProcessData(outputPlotPath, numLags, tau, normAutocorrFlag);

                List<double> xValues = processor.XList;
                List<double> yValues = processor.YList;
                ListPairs referenceLines = processor.ReferenceLines;

                Console.WriteLine("Drawing loop started!");
                plotter.AddCurve("Figure-7", xValues, yValues, Color.Black);

                //string regressionLineKey = "RegressionLine";
                //List<double> regressionYList = referenceLines[regressionLineKey].Item2;
                //plotter.AddCurve(regressionLineKey, xValues, regressionYList, Color.Purple);
                //
                string Slope2LineKey = "Slope2Line";
                List<double> slope2LineYList = referenceLines[Slope2LineKey].Item2;
                plotter.AddCurve(Slope2LineKey, xValues, slope2LineYList, Color.Green, IsSymbolVisible:false);

                FileWriter.WriteToFile(outputPlotPath, $"Figure7data_{dirDateTime}.txt", xValues, yValues);
                //FileWriter.WriteToFile(outputPlotPath, $"regressionYList_{dirDateTime}.txt", xValues, regressionYList);
                FileWriter.WriteToFile(outputPlotPath, $"slope2LineYList_{dirDateTime}.txt", xValues, slope2LineYList);

                plotter.SavePlot(outputPlotPath, $"Figure-7_{dirDateTime}.png");
                //plotter.ShowDialog();

                Console.WriteLine("Drawing loop ended!");
                Console.WriteLine("All graphs saved to " + outputPlotPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.WriteLine("Press ENTER to exit ...");
            Console.ReadKey();
        }
    }
}
