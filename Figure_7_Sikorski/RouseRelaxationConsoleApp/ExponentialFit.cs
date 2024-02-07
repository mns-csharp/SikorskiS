using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics;
using System.Drawing;
using MathNet.Numerics.LinearRegression;

namespace Figure_7_Sikorski
{
    public class CurveFunctions
    {
        private const double TINY = 0.0000001;
        private const double G_A_MIN = -100;
        private const double G_A_MAX = 100;
        private const double G_B_MIN = -10;
        private const double G_B_MAX = 10;
        private const double G_C_MIN = -10;
        private const double G_C_MAX = 10;

        public static List<PointF> Convert(List<double> XList, List<double> yList)
        {
            List<PointF> points = new List<PointF>();
            for (int i = 0; i < XList.Count; i++)
            {
                points.Add(new PointF((float)XList[i], (float)yList[i]));
            }
            return points;
        }

        // Return the function for x, a, b, and c.
        public static double F(double x, double a, double b, double c)
        {
            return a + b * Math.Exp(c * x);
        }

        // dF/da
        private static double dFda(double x, double a, double b, double c)
        {
            return 1;
        }

        // dF/db
        private static double dFdb(double x, double a, double b, double c)
        {
            return Math.Exp(c * x);
        }

        // dF/dc
        private static double dFdc(double x, double a, double b, double c)
        {
            return b * Math.Exp(c * x) * x;
        }

        // Return the sum of errors squared for the given points and parameters.
        public static double ErrorSquared(List<PointF> pts, double a, double b, double c)
        {
            double total_error = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                double new_error = pts[i].Y - F(pts[i].X, a, b, c);
                if (total_error > 1E+15) break;
                total_error += new_error * new_error;
            }

            return total_error;
        }

        // dE/da
        private static double dEda(List<PointF> pts, double a, double b, double c)
        {
            double total = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                total += 2 * (pts[i].Y - F(pts[i].X, a, b, c)) *
                    -dFda(pts[i].X, a, b, c);
            }
            return total;
        }

        // dE/db
        private static double dEdb(List<PointF> pts, double a, double b, double c)
        {
            double total = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                total += 2 * (pts[i].Y - F(pts[i].X, a, b, c)) *
                    -dFdb(pts[i].X, a, b, c);
            }
            return total;
        }

        // dE/dc
        private static double dEdc(List<PointF> pts, double a, double b, double c)
        {
            double total = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                total += 2 * (pts[i].Y - F(pts[i].X, a, b, c)) *
                    -dFdc(pts[i].X, a, b, c);
            }
            return total;
        }

        // Find a good curve fit.
        public static double FindGoodFit(List<PointF> pts,
            out double best_a, out double best_b, out double best_c,
            int num_steps, int max_iterations)
        {
            best_a = 0;
            best_b = 0;
            best_c = 0;
            double test_a = 0, test_b = 0, test_c = 0;
            double g_a_step = (G_A_MAX - G_A_MIN) / (num_steps - 1);
            double g_b_step = (G_B_MAX - G_B_MIN) / (num_steps - 1);
            double g_c_step = (G_C_MAX - G_C_MIN) / (num_steps - 1);

            double best_error2 = double.MaxValue;
            for (double a = G_A_MIN; a <= G_A_MAX; a += g_a_step)
            {
                for (double b = G_B_MIN; b <= G_B_MAX; b += g_b_step)
                {
                    for (double c = G_C_MIN; c <= G_C_MAX; c += g_c_step)
                    {
                        double test_error2 = FollowGradient(pts, a, b, c,
                            out test_a, out test_b, out test_c, max_iterations);
                        if (test_error2 < best_error2)
                        {
                            best_a = test_a;
                            best_b = test_b;
                            best_c = test_c;
                            best_error2 = test_error2;
                        }
                    }
                }
            }

            return Math.Sqrt(best_error2);
        }

        // Starting at (X, Y), follow the error
        // function's gradient to try to improve it.
        private static double FollowGradient(List<PointF> pts,
            double a, double b, double c,
            out double best_a, out double best_b, out double best_c,
            int max_iterations)
        {
            const double CUTOFF_ERROR = 1.0;

            // Set the initial distance to move.
            double dist = 0.25;

            // Start with the initial point.
            best_a = a;
            best_b = b;
            best_c = c;
            double best_error2 = ErrorSquared(pts, a, b, c);

            for (int iteration = 0; iteration < max_iterations; iteration++)
            {
                // Get the gradient at this point.
                double na = dEda(pts, a, b, c);
                double nb = dEdb(pts, a, b, c);
                double nc = dEdc(pts, a, b, c);

                // Normalize it.
                double length = na * na + nb * nb + nc * nc;
                length = Math.Sqrt(length);
                // If the length is too small, return the result so far.
                if (length < TINY) return best_error2;
                na = -na / length;
                nb = -nb / length;
                nc = -nc / length;

                // Try moving along the gradient.
                double va = na * dist;
                double vb = nb * dist;
                double vc = nc * dist;
                double test_error2 = ErrorSquared(pts, a + va, b + vb, c + vc);

                // Try increasingly smaller vectors
                // until we find an improvement.
                while (test_error2 > best_error2)
                {
                    dist /= 2;
                    if (dist < TINY)
                    {
                        // We're not moving far enough. Stop.
                        // Return the result so far.
                        return best_error2;
                    }
                    va = na * dist;
                    vb = nb * dist;
                    vc = nc * dist;
                    test_error2 = ErrorSquared(pts, a + va, b + vb, c + vc);
                }

                // Hopefully at this point we have found an improvement.
                a += va;
                b += vb;
                c += vc;
                best_a = a;
                best_b = b;
                best_c = c;
                best_error2 = test_error2;

                // If the error is small enough, return.
                if (best_error2 < CUTOFF_ERROR) return best_error2;
            } // Next iteration.

            // Return the error.
            return best_error2;
        }
    }
    


    public class ExponentialFit
    {
        public static (double A0, double tau0)? GetExponentialFit(List<double> x, List<double> y)
        {
            try
            {
                if (x.Count != y.Count)
                    throw new ArgumentException("The count of x and y values must be the same.");

                if (x.Count <= 1 || y.Count <= 1)
                    throw new ArgumentException("The count of x and y values must be more than one.");

                // Check for non-positive values in y and adjust if necessary
                double minPositiveValue = y.FindAll(val => val > 0).DefaultIfEmpty(0).Min();
                double offset = (minPositiveValue <= 0) ? (1.0 - minPositiveValue) : 0;

                // Transform the y values with the natural logarithm, applying offset to avoid log(0)
                double[] lnY = y.Select(val => Math.Log(val + offset)).ToArray();

                // Perform a polynomial regression fit to the transformed data
                // This will return the coefficients of the polynomial that best fits the data
                double[] p = Fit.Polynomial(x.ToArray(), lnY, 3); // degree 1 for linear polynomial

                // p[0] is the constant term (intercept) and p[1] is the linear coefficient (slope)
                double A0 = Math.Exp(p[0]) - offset; // Adjust back A0 if an offset was applied
                double B = (-1)*p[1]; // The slope of the line is the rate of the exponential growth/decay

                return (A0, B);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }

        public static (double A0, double tau0)? GetExponentialFit0(List<double> x, List<double> y)
        {
            var tuple = Fit.Exponential(x.ToArray(), y.ToArray(), DirectRegressionMethod.Svd);

            return (tuple.Item1, (-1)*tuple.Item2);
        }

        public static (double A0, double tau0)? GetExponentialFit1(List<double> x, List<double> y)
        {
            try
            {
                if (x.Count != y.Count)
                    throw new ArgumentException("The count of x and y values must be the same.");

                if (!(x.Count > 1 && y.Count > 1))
                    throw new ArgumentException("The count of x and y values must have more than two items.");

                // Check for non-positive values in y and adjust if necessary
                double minPositiveValue = y.FindAll(val => val > 0).DefaultIfEmpty(0).Min();
                double offset = (minPositiveValue <= 0) ? (1.0 - minPositiveValue) : 0;

                // Transform the y values with the natural logarithm, applying offset to avoid log(0)
                List<double> lnY = y.ConvertAll(val => Math.Log(val + offset));

                // Perform a linear regression fit to the transformed data
                var p = Fit.LinearCombination(x.ToArray(), lnY.ToArray(), t => 1.0, t => -t);

                // p[0] represents the intercept, which is the logarithm of A0
                // p[1] represents the slope, which is negative b
                double A0 = Math.Exp(p[0]) - offset; // Adjust back A0 if an offset was applied
                double b = p[1]; // Note the minus sign to ensure b is positive

                return (A0, b);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as appropriate
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return null;
        }

        public static (double A0, double tau0)? GetExponentialFit2(List<double> xData, List<double> yData)
        {
            if (xData == null || yData == null)
            {
                throw new ArgumentNullException("[Exponential Fit]: lists cannot be null");
            }

            if (xData.Count <= 0 || yData.Count <= 0)
            {
                throw new ArgumentException("[Exponential Fit]: lists cannot be empty");
            }

            if (xData.Count != yData.Count)
            {
                throw new ArgumentException("[Exponential Fit]: lists must be of same size");
            }

            // Transform the yData for linear regression using logarithms:
            // If y = a * exp(b * x), then log(y) = log(a) + b * x.
            List<double> logYData = new List<double>();
            for (int i = 0; i < yData.Count; i++)
            {
                logYData.Add(Math.Log(yData[i]));
            }

            // Perform linear regression on the transformed data
            var linearFit = SimpleRegression.Fit(xData.ToArray(), logYData.ToArray());

            // Extract the coefficients from the linear fit
            double A0 = linearFit.Item2; // This is the exponential decay rate
            double tau0 = Math.Exp(linearFit.Item1); // Convert back from log scale

            return (A0, tau0);
        }
    }
}
