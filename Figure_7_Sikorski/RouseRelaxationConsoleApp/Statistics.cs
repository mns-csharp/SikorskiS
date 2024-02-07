using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;

namespace Figure_7_Sikorski
{
    public class Statistics
    {
        private List<double> data_;
        private double? mean_ = null;
        private double? stdDev_ = null;
        private double? variance_ = null;

        #region [ctor]
        public Statistics(List<double> data)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Data cannot be null or empty.", nameof(data));
            data_ = data;

            GetMean();
            GetVariance();
        }
        #endregion

        #region [Autocorrelation functions]
        ////public double AutocorrelationCoeff(int lag)
        ////{
        ////    if (lag >= data_.Count || lag < 0)
        ////        throw new ArgumentException("Lag is out of range.", nameof(lag));
        ////
        ////    var laggedData = data_.Skip(lag).ToList();
        ////    var originalData = data_.Take(data_.Count - lag).ToList();
        ////    return Correlation.Pearson(originalData, laggedData);
        ////}

        ////public (List<double>, List<double>) AutoCorrelationPoints(double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        ////{
        ////    int n = data_.Count;
        ////    double mean = data_.Average();
        ////    double var = data_.Sum(d => (d - mean) * (d - mean));
        ////    List<double> tauList = new List<double>();
        ////    List<double> autoCorrList = new List<double>();
        ////
        ////    for (int lag = 1; lag < Math.Min(n, numLag); lag++)
        ////    {
        ////        double cov = 0.0;
        ////        for (int i = 0; i < n - lag; i++)
        ////        {
        ////            cov += (data_[i] - mean) * (data_[i + lag] - mean);
        ////        }
        ////        double autocorrelation = cov / var;
        ////        if (autocorrelation > thresholdMin && autocorrelation < thresholdMax)
        ////        {
        ////            tauList.Add(lag);
        ////            autoCorrList.Add(autocorrelation);
        ////        }
        ////    }
        ////    return (tauList, autoCorrList);
        ////}
        #endregion

        #region [common statistics functions]
        public double GetMean()
        {
            mean_ = data_.Mean();
            return mean_.Value;
        }

        public double GetVariance()
        {
            variance_ = data_.Variance();
            return variance_.Value;
        }

        public double GetStdDev()
        {
            stdDev_ = data_.StandardDeviation();
            return stdDev_.Value;
        }

        public double CorrelationCoeff(Statistics other)
        {
            if (data_.Count != other.data_.Count)
                throw new ArgumentException("Datasets must be of equal length.");

            return Correlation.Pearson(data_, other.data_);
        }
        #endregion
        
        public (List<double> lags, List<double> autocorrelationValues) AutoCorrelationPointsR2(
double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        {
            var autocorrelationValues = new List<double>();
            var lags = new List<double>();
            int n = data_.Count;
            double varianceValue = variance_.GetValueOrDefault();

            if (numLag > n)
            {
                numLag = n; // Ensure the number of lags does not exceed the data count.
            }

            // Subtract mean and prepare data for autocorrelation calculation
            double[] centeredData = data_.Select(x => x - mean_.Value).ToArray();
            double[] autocorrelations = new double[numLag];

            // Parallelize the autocorrelation calculations
            Parallel.For(0, numLag, t =>
            {
                double sumProduct = 0.0;

                // Use a running sum to calculate the sum of products for the current lag.
                if (t == 0)
                {
                    // For lag 0, the sum of products is simply the sum of squared centered data.
                    sumProduct = centeredData.Sum(x => x * x);
                }
                else
                {
                    // For lag t, use the previous sumProduct and adjust for the new lag.
                    for (int i = 0; i < n - t; i++)
                    {
                        sumProduct += centeredData[i] * centeredData[i + t];
                    }
                }

                double autocorrValue = sumProduct / (n - t);

                // Normalize by variance if required, and check thresholds
                if (varianceValue > 0)
                {
                    autocorrValue /= varianceValue;
                }

                autocorrelations[t] = autocorrValue;
            });

            for (int t = 0; t < numLag; t++)
            {
                double autocorrValue = autocorrelations[t];
                if (autocorrValue >= thresholdMin && autocorrValue <= thresholdMax)
                {
                    autocorrelationValues.Add(autocorrValue);
                    lags.Add(t);
                }
            }

            return (lags, autocorrelationValues);
        }

        #region [Autocorrelation functions]
        //public (List<double> lags, List<double> autocorrelationValues) AutoCorrelationPointsR2(double thresholdMin = 0.0001, double thresholdMax = 0.9999, int numLag = 1000)
        //{
        //    var autocorrelationValues = new List<double>();
        //    var lags = new List<double>();
        //    int n = data_.Count;

        //    // Subtract mean and prepare data for autocorrelation calculation
        //    List<double> centeredData = data_.Select(x => x - mean_.Value).ToList();

        //    // Calculate autocorrelation for each lag
        //    for (int t = 0; t < numLag && t < n; t++)
        //    {
        //        double sumProduct = 0.0;
        //        for (int i = 0; i < n - t; i++)
        //        {
        //            sumProduct += centeredData[i] * centeredData[i + t];
        //        }

        //        double autocorrValue = sumProduct / (n - t);
        //        // Normalize by variance if required, and check thresholds
        //        if (variance_.HasValue && variance_.Value > 0)
        //        {
        //            autocorrValue /= variance_.Value;
        //        }

        //        if (autocorrValue >= thresholdMin && autocorrValue <= thresholdMax)
        //        {
        //            autocorrelationValues.Add(autocorrValue);
        //            lags.Add(t);
        //        }
        //    }

        //    return (lags, autocorrelationValues);
        //}
        #endregion


        public bool IsConverged(int numberOfBlocks = 5)
        {
            // Call the method to perform block averaging
            var blockAverages = PerformBlockAveraging(data_, numberOfBlocks);

            // Output the results
            Console.WriteLine("Block Averages for r^2 Data:");
            for (int i = 0; i < blockAverages.Count; i++)
            {
                Console.WriteLine($"Block {i + 1}: Average r^2 = {blockAverages[i]}");
            }

            // Check for consistency
            double globalAverage = this.GetMean();
            bool isConsistent = blockAverages.All(avg => Math.Abs(avg - globalAverage) < globalAverage * 0.1); // 10% threshold, adjust as needed

            return isConsistent;
        }

        private List<double> PerformBlockAveraging(List<double> data, int numberOfBlocks)
        {
            int blockSize = data.Count / numberOfBlocks;
            var blockAverages = new List<double>();

            for (int i = 0; i < numberOfBlocks; i++)
            {
                // Calculate the start and end indices for the current block
                int start = i * blockSize;
                int end = (i == numberOfBlocks - 1) ? data.Count : start + blockSize;
                double blockAverage = data.GetRange(start, end - start).Average();
                blockAverages.Add(blockAverage);
            }

            return blockAverages;
        }
    }
}