using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Interpolation;

namespace Figure_7_Sikorski
{
    public static class MathUtils
    {
        #region [GetExponentialDecayLine]
        public static (List<double>, List<double>) GetExponentialDecayLine(int minX, int maxX, double A, double b)
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            // Ensure minX is not greater than maxX
            if (minX > maxX)
            {
                throw new ArgumentException("minX must be less than or equal to maxX");
            }

            for (double x = minX; x <= maxX; x = x + 1)
            {
                double y = Math.Abs(A) * Math.Exp((-1) * Math.Abs(x) * Math.Abs(b) * 10000);

                xValues.Add(x);
                yValues.Add(y);
            }

            return (xValues, yValues);
        }
        #endregion

        #region [LineSpace]
        /// <summary>
        /// Generates a sequence of numbers linearly spaced between two specified numbers.
        /// </summary>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="num">The number of values to generate.</param>
        /// <returns>An array of linearly spaced numbers.</returns>
        public static List<double> LineSpace(double start, double end, int num)
        {
            // MathNet.Numerics provides a direct method to generate linearly spaced arrays.
            return new List<double>(Generate.LinearSpaced(num, start, end));
        }
        #endregion

        #region [Interp]
        public static List<double> Interp(List<double> xCommon, List<double> xOriginal, List<double> yOriginal)
        {
            // Use MathNet.Numerics to create a linear spline interpolation
            var interpolation = LinearSpline.InterpolateSorted(xOriginal.ToArray(), yOriginal.ToArray());

            // Evaluate the interpolation at each point in xCommon
            List<double> yInterpolated = new List<double>();
            foreach (double x in xCommon)
            {
                yInterpolated.Add(interpolation.Interpolate(x));
            }

            return yInterpolated;
        }
        #endregion

        #region [FindIntersectionOfTwoCurves]
        public static Vec2 GetIntersectionOfTwoCurves(List<double> xList1, List<double> yList1,
                                               List<double> xList2, List<double> yList2)
        {
            var returns = IntersectionFinder.FindIntersectionOfTwoCurves(xList1, yList1, xList2, yList2);


            if (returns.HasValue)
            {
                return new Vec2(returns.Value.Item1, returns.Value.Item2);
            }

            return null;
        }
        #endregion
    }
}