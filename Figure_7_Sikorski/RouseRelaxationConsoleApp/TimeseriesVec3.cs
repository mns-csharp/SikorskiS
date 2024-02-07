using System.Collections.Generic;
using System;
using System.Threading.Tasks;


namespace Figure_7_Sikorski
{
    public class TimeSeriesAutoCorr
    {
        private static double AutocorrelationFunction(List<Vector3> vectors, int t)
        {
            if (vectors == null || vectors.Count == 0)
            {
                throw new ArgumentException("The vector array must not be null or empty");
            }

            if (t < 0 || t >= vectors.Count)
            {
                throw new ArgumentException("Time shift 't' must be non-negative and less than the number of vectors");
            }

            int n = vectors.Count;
            double sum = 0.0;

            for (int i = 0; i < n - t; i++)
            {
                sum += Vector3.Dot(vectors[i], vectors[i + t]);
            }

            double autocorrelation = sum / (n - t);

            return autocorrelation;
        }


        public static (List<double> lags, List<double> autocorrelationValues) AutocorrelationList(List<Vector3> vectors, 
            double minThreshold=0.00001, double maxThresholr=0.9999, int maxLag = 1000)
        {
            List<double> lags = new List<double>();
            List<double> autocorrelations = new List<double>();
            for (int tau = 0; tau < maxLag; tau++)
            {
                double autocorrVal = AutocorrelationFunction(vectors, tau);

                if (autocorrVal >= minThreshold && autocorrVal <= maxThresholr)
                {
                    lags.Add(tau);
                    autocorrelations.Add(autocorrVal);
                }
            }

            return (lags, autocorrelations);
        }

        public static (List<double> lags, List<double> autocorrelationValues) AutocorrelationListNprmalized1(List<Vector3> vectors,
            double minThreshold = 0.00001, double maxThreshold = 0.9999, int maxLag = 1000)
        {
            List<double> lags = new List<double>();
            List<double> autocorrelations = new List<double>();

            // First, calculate the autocorrelation at lag 0 for normalization
            double autocorrAtLagZero = AutocorrelationFunction(vectors, 0);

            if (autocorrAtLagZero == 0)
            {
                throw new InvalidOperationException("Autocorrelation at lag 0 is zero, cannot normalize.");
            }

            for (int tau = 0; tau < maxLag; tau++)
            {
                double autocorrVal = AutocorrelationFunction(vectors, tau) / autocorrAtLagZero;

                if (tau == 998)
                {
                    string strings = string.Empty;
                }

                if (autocorrVal >= minThreshold && autocorrVal <= maxThreshold)
                {
                    lags.Add(tau);
                    autocorrelations.Add(autocorrVal);
                }
            }

            return (lags, autocorrelations);
        }
    }
}
