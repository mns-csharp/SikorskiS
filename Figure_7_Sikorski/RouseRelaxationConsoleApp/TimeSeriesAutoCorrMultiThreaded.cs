using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public class TimeSeriesAutoCorrMultiThreaded
    {
        // Forward computation optimization: Pre-calculate dot products
        private static double[] PreComputeDotProducts(List<Vector3> vectors)
        {
            int n = vectors.Count;
            double[] dotProducts = new double[n * (n + 1) / 2];
            int index = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    dotProducts[index++] = Vector3.Dot(vectors[i], vectors[j]);
                }
            }

            return dotProducts;
        }

        // Adjusted AutocorrelationFunction to use pre-computed dot products
        private static double AutocorrelationFunction(double[] dotProducts, int t, int n)
        {
            double sum = 0.0;
            int counter = 0;

            for (int i = 0; i < n - t; i++)
            {
                sum += dotProducts[i * (2 * n - i - 1) / 2 + t];
                counter++;
            }

            double autocorrelation = sum / counter;
            return autocorrelation;
        }

        // Multithreading optimization: Use parallel processing to compute autocorrelation list
        public static (List<double> lags, List<double> autocorrelationValues) AutocorrelationList(List<Vector3> vectors,
            double minThreshold = 0.00001, double maxThreshold = 0.9999, int maxLag = 1000)
        {
            double[] dotProducts = PreComputeDotProducts(vectors);
            int n = vectors.Count;
            List<double> lags = new List<double>();
            List<double> autocorrelations = new List<double>();

            Parallel.For(0, maxLag, tau =>
            {
                double autocorrVal = AutocorrelationFunction(dotProducts, tau, n);

                if (autocorrVal >= minThreshold && autocorrVal <= maxThreshold)
                {
                    lock (lags)
                    {
                        lags.Add(tau);
                    }
                    lock (autocorrelations)
                    {
                        autocorrelations.Add(autocorrVal);
                    }
                }
            });

            return (lags, autocorrelations);
        }

        // Multithreading optimization applied to the normalized autocorrelation list method
        public static (List<double> lags, List<double> autocorrelationValues)
            AutocorrelationListNormalized1(List<Vector3> vectors,
            double minThreshold = 0.00001, double maxThreshold = 0.9999, int maxLag = 1000)
        {
            double[] dotProducts = PreComputeDotProducts(vectors);
            int n = vectors.Count;
            List<double> lags = new List<double>();
            List<double> autocorrelations = new List<double>();

            double autocorrAtLagZero = AutocorrelationFunction(dotProducts, 0, n);

            if (autocorrAtLagZero == 0)
            {
                throw new InvalidOperationException("Autocorrelation at lag 0 is zero, cannot normalize.");
            }

            Parallel.For(0, maxLag, tau =>
            {
                double autocorrVal = AutocorrelationFunction(dotProducts, tau, n) / autocorrAtLagZero;

                if (autocorrVal >= minThreshold && autocorrVal <= maxThreshold)
                {
                    lock (lags)
                    {
                        lags.Add(tau);
                    }
                    lock (autocorrelations)
                    {
                        autocorrelations.Add(autocorrVal);
                    }
                }
            });

            return (lags, autocorrelations);
        }
    }
}
