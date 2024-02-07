using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using NonLinearRegressionCurveFittingTesting;


namespace Figure_7_Sikorski
{
    public class RvaluesProcessor
    {
        public string SimulationID { get; private set; }
        public string ObservableID { get; private set; }
        public List<double> TauListX { get; private set; } = new List<double>();
        public List<double> AutoCorrelationListY { get; private set; } = new List<double>();
        public double ResidueLength { get; private set; }
        public double A0Value { get; private set; }
        public double Tau0Value { get; private set; }

        public RvaluesProcessor(string simulationID)
        {
            SimulationID = simulationID;
        }

        public void ProcessData(string inputPath, string outputPath, int maxNumLags, int lag, bool normAutocorrFlag)
        {
            ObservableID = SimulationID + "_" + Path.GetFileName(inputPath);

            ResidueLength = GetResidueCount();

            if (Settings.DataFileType == DataFileTypeEnum.R2)
            {
                #region R2.dat
                List<double> dataDoubleList = FileReader.ReadOneColumn(inputPath, Settings.DataFileName);

                if (dataDoubleList == null || !dataDoubleList.Any())
                {
                    throw new ArgumentException("Data cannot be null or empty.", nameof(dataDoubleList));
                }

                // Generate autocorrelation values
                Statistics stats = new Statistics(dataDoubleList);
                var autocorrelation = stats.AutoCorrelationPointsR2(Settings.AutoCorrThresholdMin,
                                                                Settings.AutoCorrThresholdMax,
                                                                Settings.NumAutoCorrLags);

                FileWriter.WriteToFile(outputPath, "stats.AutoCorrelationPointsR2.txt", autocorrelation.autocorrelationValues);

                TauListX = autocorrelation.lags;
                AutoCorrelationListY = autocorrelation.autocorrelationValues;

                if (Settings.IsConvertYtoLogY)
                {
                    AutoCorrelationListY = ListUtils.ToLog(autocorrelation.Item2);
                }
                else
                {
                    AutoCorrelationListY = autocorrelation.Item2;
                }

                double[] x = TauListX.ToArray();
                double[] y = AutoCorrelationListY.ToArray();

                var fittedCurve = NonlinearRegressionCurveFitting.Fit(new List<double>(x), new List<double>(y));

                List<double> xExpDecayList = fittedCurve.XFit;
                List<double> yExpDecayList = fittedCurve.YFit;
                List<double> results = fittedCurve.MinimizingPoint;

                if (Settings.IsConvertYtoLogY)
                {
                    A0Value = Math.Exp(results[0]);
                }
                else
                {
                    A0Value = results[0];
                }

                Tau0Value = 1.0 / ((-1.0) * results[1]);

                FileWriter.WriteToFile(outputPath, $"A0_vs_tau0.txt", $"{A0Value}\t{Tau0Value}");

                DataPlotter plotter = new DataPlotter();
                plotter.IsLogX = false;
                plotter.IsLogY = false;

                plotter.AddCurve("AutoCorrelation", TauListX, AutoCorrelationListY, Color.Red, IsSymbolVisible: false);
                plotter.AddCurve("ExponentialDecay", xExpDecayList, yExpDecayList, Color.Orange, IsSymbolVisible: false);
                Console.WriteLine($"Data saved to: {outputPath}");
                plotter.SavePlot(outputPath, $"fit_{ObservableID}.png");

                FileWriter.WriteToFile(outputPath, $"autocorr_{ObservableID}.txt", TauListX, AutoCorrelationListY);
                FileWriter.WriteToFile(outputPath, $"exp_decay_{ObservableID}.txt", xExpDecayList, yExpDecayList); 
                #endregion
            }
            else if (Settings.DataFileType == DataFileTypeEnum.RendVec)
            {
                List<Vector3> dataVec3dList = FileReader.ReadVec3(inputPath, Settings.DataFileName);

                if (dataVec3dList == null || !dataVec3dList.Any())
                {
                    throw new ArgumentException("Data cannot be null or empty.", nameof(dataVec3dList));
                }

                // Calculate the mean of the end-to-end vectors for normalization
                (List<double> lags, List<double> autocorrelationValues) autocorrelation = (new List<double>(), new List<double>());

                if (Settings.Parallelize)
                {
                    if (!Settings.NormAutoCorr)
                    {
                        autocorrelation = TimeSeriesAutoCorrMultiThreaded.AutocorrelationList(dataVec3dList,
                                                                   Settings.AutoCorrThresholdMin,
                                                                   Settings.AutoCorrThresholdMax,
                                                                   Settings.NumAutoCorrLags);
                    }
                    else if (Settings.NormAutoCorr)
                    {
                        autocorrelation = TimeSeriesAutoCorrMultiThreaded.AutocorrelationListNormalized1(dataVec3dList,
                                                                   Settings.AutoCorrThresholdMin,
                                                                   Settings.AutoCorrThresholdMax,
                                                                   Settings.NumAutoCorrLags);
                    }
                }
                else
                {
                    if (!Settings.NormAutoCorr)
                    {
                        autocorrelation = TimeSeriesAutoCorr.AutocorrelationList(dataVec3dList,
                                                                   Settings.AutoCorrThresholdMin,
                                                                   Settings.AutoCorrThresholdMax,
                                                                   Settings.NumAutoCorrLags);
                    }
                    else if (Settings.NormAutoCorr)
                    {
                        autocorrelation = TimeSeriesAutoCorr.AutocorrelationListNprmalized1(dataVec3dList,
                                                                   Settings.AutoCorrThresholdMin,
                                                                   Settings.AutoCorrThresholdMax,
                                                                   Settings.NumAutoCorrLags);
                    }
                }

                FileWriter.WriteToFile(outputPath, "Vector3.AutoCorrelationVec3.txt", autocorrelation.autocorrelationValues);

                TauListX = new List<double>( autocorrelation.lags);
                AutoCorrelationListY = new List<double>(autocorrelation.autocorrelationValues);

                // ... rest of your code ...
                double[] x = TauListX.ToArray();
                double[] y = AutoCorrelationListY.ToArray();

                var fittedCurve = NonlinearRegressionCurveFitting.Fit(new List<double>(x), new List<double>(y));

                List<double> xExpDecayList = fittedCurve.XFit;
                List<double> yExpDecayList = fittedCurve.YFit;
                List<double> results = fittedCurve.MinimizingPoint;

                if (Settings.IsConvertYtoLogY)
                {
                    A0Value = Math.Exp(results[0]);
                }
                else
                {
                    A0Value = results[0];
                }

                Tau0Value = 1.0 / ((-1.0) * results[1]);

                FileWriter.WriteToFile(outputPath, $"A0_vs_tau0.txt", $"{A0Value}\t{Tau0Value}");

                DataPlotter plotter = new DataPlotter();
                plotter.IsLogX = false;
                plotter.IsLogY = false;

                plotter.AddCurve("AutoCorrelation", TauListX, AutoCorrelationListY, Color.Red, IsSymbolVisible: false);
                plotter.AddCurve("ExponentialDecay", xExpDecayList, yExpDecayList, Color.Orange, IsSymbolVisible: false);
                Console.WriteLine($"Data saved to: {outputPath}");
                plotter.SavePlot(outputPath, $"fit_{ObservableID}.png");

                FileWriter.WriteToFile(outputPath, $"autocorr_{ObservableID}.txt", TauListX, AutoCorrelationListY);
                FileWriter.WriteToFile(outputPath, $"exp_decay_{ObservableID}.txt", xExpDecayList, yExpDecayList);
            }
        }

        private void NormalizeData()
        {
            double maxY = AutoCorrelationListY.Max();
            double minY = AutoCorrelationListY.Min();
            for (int i = 0; i < AutoCorrelationListY.Count; i++)
            {
                AutoCorrelationListY[i] = (AutoCorrelationListY[i] - minY) / (maxY - minY);
            }
        }

        private int GetResidueCount()
        {
            var match = Regex.Match(ObservableID, @"residue(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                throw new ArgumentException("Residue number not found in directory path.");
            }
        }
    }
}