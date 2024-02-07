using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Figure_7_Sikorski
{
    public static class FitInLogScale
    {
        public static Tuple<List<double>, List<double>> GetNewLineOfSlopeIntersectingGivenLine(double newSlope, List<double> xList, List<double> yList)
        {
            // Desired slope for the new line in log-log space
            double desiredSlope = newSlope;

            // Ensure all x-values are positive and greater than zero for logarithmic scale
            xList = xList.Where(x => x > 0).ToList();
            yList = yList.Where(y => y > 0).ToList(); // Also ensure y-values are positive

            if (!xList.Any() || !yList.Any()) return null;

            // Transform to log space for fitting
            List<double> logXList = xList.Select(Math.Log10).ToList();
            List<double> logYList = yList.Select(Math.Log10).ToList();

            // Calculate the midpoint of the regression line in log space
            double middleXLog = (logXList.Min() + logXList.Max()) / 2;
            double middleYLog = (logYList.Min() + logYList.Max()) / 2;

            // Calculate the y-intercept of the new line in log space
            double yInterceptLog = middleYLog - (desiredSlope * middleXLog);

            // Create the new line in log space using the desired slope and calculated y-intercept
            List<double> newLogXValues = new List<double>(logXList);
            List<double> newLogYValues = newLogXValues.Select(x => (desiredSlope * x) + yInterceptLog).ToList();

            // Transform back from log space to linear space
            // Transform back from log space to linear space
            List<double> newXValues = newLogXValues.Select(x => Math.Pow(10, x)).ToList();
            List<double> newYValues = newLogYValues.Select(y => Math.Pow(10, y)).ToList();

            return Tuple.Create(newXValues, newYValues);
        }

        #region Regression Helper
        private static Tuple<double, double> CalculateLogarithmicRegressionCoefficients(double[] xValues, double[] yValues)
        {
            if (xValues.Length != yValues.Length)
            {
                throw new ArgumentException("Input arrays must have the same length.");
            }

            var logXValues = xValues.Select(x => Math.Log(x)).ToArray();
            var logYValues = yValues.Select(y => Math.Log(y)).ToArray();

            var (a, b) = LinearRegression(logXValues, logYValues);
            return Tuple.Create(a, b);
        }

        private static Tuple<double, double> LinearRegression(double[] xVals, double[] yVals)
        {
            if (xVals.Length != yVals.Length)
            {
                throw new InvalidOperationException("The number of x and y values must be equal.");
            }

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            int n = xVals.Length;

            for (int i = 0; i < n; i++)
            {
                sumX += xVals[i];
                sumY += yVals[i];
                sumXY += xVals[i] * yVals[i];
                sumX2 += xVals[i] * xVals[i];
            }

            double xMean = sumX / n;
            double yMean = sumY / n;
            double denominator = sumX2 - sumX * xMean;

            if (denominator == 0)
            {
                throw new InvalidOperationException("Denominator in linear regression calculation is zero.");
            }

            double b = (sumXY - sumX * yMean) / denominator;
            double a = yMean - b * xMean;

            return Tuple.Create(a, b);
        }
        #endregion

        public static Tuple<List<double>, List<double>> CreateRegressionLine(List<double> xList, List<double> yList)
        {
            var xValues = xList.ToArray();
            var yValues = yList.ToArray();
            var coefficients = CalculateLogarithmicRegressionCoefficients(xValues, yValues);

            var regressionYValues = xList.Select(x => Math.Exp(coefficients.Item1) * Math.Pow(x, coefficients.Item2)).ToList();

            return Tuple.Create(xList, regressionYValues);
        }
    }
}
