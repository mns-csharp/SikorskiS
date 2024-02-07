using NonLinearRegressionCurveFittingTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonLinearRegressionCurveFittingTesting
{
    class NumericalAnalysisToolBoxCSharp
    {
        #region LeastSquaresBestFitLine1
        public static double[] LeastSquaresBestFitLine1(double[] x, double[] logY)
        {
            //Calculates equation of best-fit line using shortcuts
            int n = x.Length;
            double xMean = 0.0;
            double yMean = 0.0;
            double numeratorSum = 0.0;
            double denominatorSum = 0.0;
            double bestfitYintercept = 0.0;
            double bestfitSlope = 0.0;
            double sigma = 0.0;
            double sumOfResidualsSquared = 0.0;
            //Calculates the mean values for x and y arrays
            for (int i = 0; i < n; i++)
            {
                xMean += x[i] / n;
                yMean += logY[i] / n;
            }
            //Calculates the numerator and denominator for best-fit slope
            for (int i = 0; i < n; i++)
            {
                numeratorSum += logY[i] * (x[i] - xMean);
                denominatorSum += x[i] * (x[i] - xMean);
            }
            //Calculate the best-fit slope and y-intercept
            bestfitSlope = numeratorSum / denominatorSum;
            bestfitYintercept = yMean - xMean * bestfitSlope;
            //Calculate the best-fit standard deviation
            for (int i = 0; i < n; i++)
            {
                sumOfResidualsSquared +=
                (logY[i] - bestfitYintercept - bestfitSlope * x[i]) *
                (logY[i] - bestfitYintercept - bestfitSlope * x[i]);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - 2));
            return new double[] { bestfitYintercept, bestfitSlope, sigma };
        }
        #endregion

        #region LeastSquaresBestFitLine2
        public static double[] LeastSquaresBestFitLine2(double[] x, double[] logY)
        {
            //Calculates equation of best-fit line using sums
            int n = x.Length;
            double xSum = 0.0;
            double ySum = 0.0;
            double xySum = 0.0;
            double xSqrSum = 0.0;
            double denominator = 0.0;
            double bNumerator = 0.0;
            double mNumerator = 0.0;
            double bestfitYintercept = 0.0;
            double bestfitSlope = 0.0;
            double sigma = 0.0;
            double sumOfResidualsSquared = 0.0;
            //calculate sums
            for (int i = 0; i < n; i++)
            {
                xSum += x[i];
                ySum += logY[i];
                xySum += x[i] * logY[i];
                xSqrSum += x[i] * x[i];
            }
            denominator = n * xSqrSum - xSum * xSum;
            bNumerator = xSqrSum * ySum - xSum * xySum;
            mNumerator = n * xySum - xSum * ySum;
            //calculate best-fit y-intercept
            bestfitYintercept = bNumerator / denominator;
            //calculate best-fit slope
            bestfitSlope = mNumerator / denominator;
            //calculate best-fit standard deviation
            for (int i = 0; i < n; i++)
            {
                sumOfResidualsSquared +=
                (logY[i] - bestfitYintercept - bestfitSlope * x[i]) *
                (logY[i] - bestfitYintercept - bestfitSlope * x[i]);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - 2));
            return new double[] { bestfitYintercept, bestfitSlope, sigma };
        }
        #endregion

        


        #region LeastSquaresWeightedBestFitLine1
        public static double[] LeastSquaresWeightedBestFitLine1(double[] x, double[] y, double[] w)
        {
            //Calculates equation of best-fit line using short cuts
            int n = x.Length;
            double wxMean = 0.0;
            double wyMean = 0.0;
            double wSum = 0.0;
            double wnumeratorSum = 0.0;
            double wdenominatorSum = 0.0;
            double bestfitYintercept = 0.0;
            double bestfitSlope = 0.0;
            double sigma = 0.0;
            double sumOfResidualsSquared = 0.0;
            //Calculates the sum of the weights w[i]
            for (int i = 0; i < n; i++)
            {
                wSum += w[i];
            }
            //Calculates the mean values for x and y arrays
            for (int i = 0; i < n; i++)
            {
                wxMean += w[i] * x[i] / wSum;
                wyMean += w[i] * y[i] / wSum;
            }
            //Calculates the numerator and denominator for best-fit slope
            for (int i = 0; i < n; i++)
            {
                wnumeratorSum += w[i] * y[i] * (x[i] - wxMean);
                wdenominatorSum += w[i] * x[i] * (x[i] - wxMean);
            }
            //Calculate the best-fit slope and y-intercept
            bestfitSlope = wnumeratorSum / wdenominatorSum;
            bestfitYintercept = wyMean - wxMean * bestfitSlope;
            //Calculate the best-fit standard deviation
            for (int i = 0; i < n; i++)
            {
                sumOfResidualsSquared +=
                w[i] * (y[i] - bestfitYintercept - bestfitSlope * x[i]) *
                (y[i] - bestfitYintercept - bestfitSlope * x[i]);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - 2));
            return new double[] { bestfitYintercept, bestfitSlope, sigma };
        }
        #endregion

        #region LeastSquaresWeightedBestFitLine2
        public static double[] LeastSquaresWeightedBestFitLine2(double[] x, double[] y, double[] w)
        {
            //Calculates equation of best-fit line using sums
            int n = x.Length;
            double wSum = 0.0;
            double wxSum = 0.0;
            double wySum = 0.0;
            double wxySum = 0.0;
            double wxSqrSum = 0.0;
            double denominator = 0.0;
            double bNumerator = 0.0;
            double mNumerator = 0.0;
            double bestfitYintercept = 0.0;
            double bestfitSlope = 0.0;
            double sigma = 0.0;
            double sumOfResidualsSquared = 0.0;
            //calculate sums
            for (int i = 0; i < n; i++)
            {
                wSum += w[i];
                wxSum += w[i] * x[i];
                wySum += w[i] * y[i];
                wxySum += w[i] * x[i] * y[i];
                wxSqrSum += w[i] * x[i] * x[i];
            }
            denominator = wSum * wxSqrSum - wxSum * wxSum;
            bNumerator = wxSqrSum * wySum - wxSum * wxySum;
            mNumerator = wSum * wxySum - wxSum * wySum;
            //calculate best-fit y-intercept
            bestfitYintercept = bNumerator / denominator;
            //calculate best-fit slope
            bestfitSlope = mNumerator / denominator;
            //calculate best-fit standard deviation
            for (int i = 0; i < n; i++)
            {
                sumOfResidualsSquared +=
                w[i] * (y[i] - bestfitYintercept - bestfitSlope * x[i]) *
                (y[i] - bestfitYintercept - bestfitSlope * x[i]);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - 2));
            return new double[] { bestfitYintercept, bestfitSlope, sigma };
        }
        #endregion

        

        #region LinearRegression
        public delegate double ModelFunction(double x);
        public static RVector LinearRegression(double[] x, double[] y, ModelFunction[] f, out double sigma)
        {
            //m = number of data points
            int m = f.Length;
            RMatrix Fmatrix = new RMatrix(m, m);
            RVector Bvector = new RVector(m);

            // n = number of linear terms in the regression equation
            int n = x.Length;

            //Calculate the B vector entries
            for (int k = 0; k < m; k++)
            {
                Bvector[k] = 0.0;
                for (int i = 0; i < n; i++)
                {
                    Bvector[k] += f[k](x[i]) * y[i];
                }
            }

            //Calculate the F matrix entries
            for (int j = 0; j < m; j++)
            {
                for (int k = 0; k < m; k++)
                {
                    Fmatrix[j, k] = 0.0;

                    for (int i = 0; i < n; i++)
                    {
                        Fmatrix[j, k] += f[j](x[i]) * f[k](x[i]);
                    }
                }
            }

            // FA = B so A = Fˆ{-1}B
            RVector Avector = GaussJordan(Fmatrix, Bvector);
            // Calculate the standard deviation to estimate error
            double sumOfResidualsSquared = 0.0;
            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < m; j++)
                {
                    sum += Avector[j] * f[j](x[i]);
                }
                sumOfResidualsSquared += (y[i] - sum) * (y[i] - sum);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - m));

            return Avector;
        }

        #endregion

        #region GaussJordan
        const double epsilon = 1.0e-500;
        // Gauss-Jordan elimination to solve Ax = b for x
        public static RVector GaussJordan(RMatrix A, RVector b)
        {
            Triangulate(A, b);
            int bSize = b.GetVectorSize;
            RVector x = new RVector(bSize);
            for (int i = bSize - 1; i >= 0; i--)
            {
                double Aii = A[i, i];
                if (Math.Abs(Aii) < epsilon)
                    throw new Exception("Diagonal element is too small!");
                x[i] = (b[i] - RVector.DotProduct(A.GetRowVector(i), x)) / Aii;
            }
            return x;
        }
        #endregion

        // Triangulate matrix A
        private static void Triangulate(RMatrix A, RVector b)
        {
            int nRows = A.GetnRows;
            for (int i = 0; i < nRows - 1; i++)
            {
                double diagonalElement = pivotGaussJordan(A, b, i);
                if (Math.Abs(diagonalElement) < epsilon)
                    throw new Exception("Diagonal element is too small!");
                for (int j = i + 1; j < nRows; j++)
                {
                    double w = A[j, i] / diagonalElement;
                    for (int k = i + 1; k < nRows; k++)
                    {
                        A[j, k] -= w * A[i, k];
                    }
                    b[j] -= w * b[i];
                }
            }
        }
        private static double pivotGaussJordan(RMatrix A, RVector b, int q)
        {
            int bSize = b.GetVectorSize;
            int c = q;
            double d = 0.0;
            for (int j = q; j < bSize; j++)
            {
                double w = Math.Abs(A[j, q]);
                if (w > d)
                {
                    d = w;
                    c = j;
                }
            }
            if (c > q)
            {
                A.SwapMatrixRow(q, c);
                b.SwapVectorEntries(q, c);
            }
            return A[q, q];
        }
        

        private static double f0(double x) { return 1.0; }
        private static double f1(double x) { return x; }
        private static double f2(double x) { return x * x; }
        private static double f3(double x) { return x * x * x; }
        
        public static RVector PolynomialFit(double[] x, double[] y, int m, out double sigma)
        {
            //m = number of data points which in this case
            //for polynomials = order or degree of polynomial P_m(x)
            m++; //minor adjust
            RMatrix Fmatrix = new RMatrix(m, m);
            RVector Bvector = new RVector(m);
            // n = number of linear terms in the regression equation
            int n = x.Length;
            //Calculate the B vector entries
            for (int k = 0; k < m; k++)
            {
                Bvector[k] = 0.0;
                for (int i = 0; i < n; i++)
                { Bvector[k] += Math.Pow(x[i], k) * y[i]; }
            }
            //Calculate the F matrix entries
            for (int j = 0; j < m; j++)
            {
                for (int k = 0; k < m; k++)
                {
                    Fmatrix[j, k] = 0.0;
                    for (int i = 0; i < n; i++)
                    { Fmatrix[j, k] += Math.Pow(x[i], j + k); }
                }
            }
            // FA = B so A = Fˆ{-1}B
            RVector Avector = GaussJordan(Fmatrix, Bvector);
            // Calculate the standard deviation to estimate error
            double sumOfResidualsSquared = 0.0;
            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < m; j++)
                { sum += Avector[j] * Math.Pow(x[i], j); }
                sumOfResidualsSquared += (y[i] - sum) * (y[i] - sum);
            }
            sigma = Math.Sqrt(sumOfResidualsSquared / (n - m));
            return Avector;
        }
    }
}
