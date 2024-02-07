using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public class LinearInterpolator
    {
        private static int BinarySearch<T>(IList<T> list, T value)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            var comp = Comparer<T>.Default;
            int lo = 0, hi = list.Count - 1;
            while (lo < hi)
            {
                int m = (hi + lo) / 2;  // this might overflow; be careful.
                if (comp.Compare(list[m], value) < 0) lo = m + 1;
                else hi = m - 1;
            }
            if (comp.Compare(list[lo], value) < 0) lo++;
            return lo;
        }

        private static int FindFirstIndexGreaterThanOrEqualTo<T>
                                  (List<T> sortedList, T key)
        {
            return BinarySearch(sortedList, key);
        }

        List<double> x_values;
        List<double> y_values;
        public LinearInterpolator(List<double> x, List<double> y)
        {
            // quick argsort
            List<int> indicies = x.AsEnumerable().Select((v, i) => new { obj = v, index = i }).OrderBy(c => c.obj).Select(c => c.index).ToList();
            x_values = indicies.Select(i => x[i]).ToList();
            y_values = indicies.Select(i => y[i]).ToList();
        }

        public double Interpolate(double x)
        {
            int index = FindFirstIndexGreaterThanOrEqualTo(x_values, x);
            if (index == 0)
            {
                return y_values[0];
            }
            double y1 = y_values[index - 1];
            double y2 = y_values[index];
            double x1 = x_values[index - 1];
            double x2 = x_values[index];
            return (x - x1) / (x2 - x1) * (y2 - y1) + y1;
        }

    }
}
