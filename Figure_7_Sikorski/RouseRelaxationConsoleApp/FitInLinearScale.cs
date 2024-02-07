using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Figure_7_Sikorski
{
    public static class FitInLinearScale
    {
        public static (List<double>, List<double>) GetFittedCurve(List<double> xList, List<double> yList, int polynomialOrder = 2)
        {
            if (xList == null || yList == null)
                throw new ArgumentNullException(nameof(xList), "The xList and yList cannot be null.");

            if (xList.Count != yList.Count)
                throw new ArgumentException("The xList and yList must have the same number of elements.");

            // Fit a polynomial of specified order to the data
            double[] coefficients = Fit.Polynomial(xList.ToArray(), yList.ToArray(), polynomialOrder);

            // Generate the fitted Y values
            List<double> fittedYList = new List<double>();
            for (int i = 0; i < xList.Count; i++)
            {
                double fittedY = Polynomial.Evaluate(xList[i], coefficients);
                fittedYList.Add(fittedY);
            }

            // Return the original X values with the fitted Y values
            return (xList, fittedYList);
        }

        public static Tuple<List<double>, List<double>> GetNewLineOfSlopeIntersectingGivenLine(double newSlope, List<double> xList, List<double> yList)
        {
            if (!xList.Any() || !yList.Any()) return null;

            // Calculate the midpoint of the given points
            double middleX = (xList.Min() + xList.Max()) / 2;
            double middleY = (yList.Min() + yList.Max()) / 2;

            // Calculate the y-intercept of the new line
            double yIntercept = middleY - (newSlope * middleX);

            // Create the new line using the desired slope and calculated y-intercept
            List<double> newYValues = xList.Select(x => (newSlope * x) + yIntercept).ToList();

            return Tuple.Create(xList, newYValues);
        }

        #region Regression Helper
        private static Tuple<double, double> CalculateLinearRegressionCoefficients(double[] xValues, double[] yValues)
        {
            if (xValues.Length != yValues.Length)
            {
                throw new ArgumentException("Input arrays must have the same length.");
            }

            var (a, b) = LinearRegression(xValues, yValues);
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
            var coefficients = CalculateLinearRegressionCoefficients(xValues, yValues);

            var regressionYValues = xList.Select(x => coefficients.Item1 + coefficients.Item2 * x).ToList();

            return Tuple.Create(xList, regressionYValues);
        }
    }
}
