using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public class IntersectionFinder
    {
        public static (double, double)? FindIntersectionOfTwoCurves(List<double> xList1, List<double> yList1,
                                                   List<double> xList2, List<double> yList2)
        {
            if (xList1 == null || yList1 == null || xList2 == null || yList2 == null ||
                xList1.Count != yList1.Count || xList2.Count != yList2.Count)
                return null;

            LinearInterpolator interpolator1 = new LinearInterpolator(xList1, yList1);
            LinearInterpolator interpolator2 = new LinearInterpolator(xList2, yList2);

            double lowerBound = Math.Max(xList1.Min(), xList2.Min());
            double upperBound = Math.Min(xList1.Max(), xList2.Max());

            if (lowerBound > upperBound) // X axes don't overlap
            {
                return null;
            }

            double diff_start = interpolator1.Interpolate(lowerBound) - interpolator2.Interpolate(lowerBound);
            double diff_end = interpolator1.Interpolate(upperBound) - interpolator2.Interpolate(upperBound);

            if ((diff_start > 0 && diff_end > 0) || (diff_start < 0 && diff_end < 0)) // intersection doesn't exist
            {
                return null;
            }

            double mid = (lowerBound + upperBound) / 2;
            double diff_mid = interpolator1.Interpolate(mid) - interpolator2.Interpolate(mid);

            int iterations = 0;
            while (Math.Abs(diff_mid) > 1e-7)
            {
                if (diff_start > diff_end) // list1 is higher
                {
                    if (diff_mid > 0) // mid is also higher, intersection in right side
                    {
                        lowerBound = mid;
                        diff_start = diff_mid;
                    }
                    else // mid is lower, intersection in left side
                    {
                        upperBound = mid;
                        diff_end = diff_mid;
                    }
                }
                else // list 2 is higher
                {
                    if (diff_mid < 0)
                    {
                        lowerBound = mid;
                        diff_start = diff_mid;
                    }
                    else
                    {
                        upperBound = mid;
                        diff_end = diff_mid;
                    }
                }
                mid = (lowerBound + upperBound) / 2;
                diff_mid = interpolator1.Interpolate(mid) - interpolator2.Interpolate(mid);
                iterations++;
                if (iterations > 10000) // prevent infinite loop if Y is discontinuous
                {
                    return null;
                }
            }

            return (mid, interpolator1.Interpolate(mid));
        }
    }

    //internal class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        List<double> xList1 = [1, 1.5, 2, 3.5, 4];
    //        List<double> xList2 = [1, 2, 3, 4, 5];
    //        List<double> yList1 = [0, 2, 4, 6, 8];
    //        List<double> yList2 = [8, 6, 4, 2, 0];
    //        Console.WriteLine(IntersectionFinder.FindIntersectionOfTwoCurves(xList1, yList1, xList2, yList2).ToString());
    //    }
    //}
}
