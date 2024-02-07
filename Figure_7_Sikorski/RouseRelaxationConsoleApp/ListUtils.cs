using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public static class ListUtils
    {
        public static Tuple<List<double>, List<double>> Sort(List<double> XList, List<double> YList)
        {
            // Sort XList and YList
            List<Tuple<double, double>> combinedList = XList.Zip(YList, (x, y) => new Tuple<double, double>(x, y)).ToList();
            combinedList.Sort((pair1, pair2) => pair1.Item1.CompareTo(pair2.Item1));
            XList = combinedList.Select(pair => pair.Item1).ToList();
            YList = combinedList.Select(pair => pair.Item2).ToList();

            return new Tuple<List<double>, List<double>>(XList, YList);
        }

        public static List<double> ToLog(List<double> yList)
        {
            List<double> yListLog = new List<double>();
            foreach (var item in yList)
            {
                double yLog = Math.Log10(item);
                yListLog.Add(item);
            }
            return yListLog;
        }
    }
}

