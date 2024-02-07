using System;
using System.Collections.Generic;
using System.Linq;

namespace Figure_7_Sikorski
{
    public class ListPairs : Dictionary<string, Tuple<List<double>, List<double>>>
    {
        public void Add(string key, List<double> list1, List<double> list2)
        {
            if (list1.Count != list2.Count)
            {
                throw new ArgumentException("Lists must be of the same length.", nameof(list1));
            }

            var tuple = new Tuple<List<double>, List<double>>(list1, list2);

            this[key] = tuple;
        }

        public List<double> GetComputeConcatenatedXList()
        {
            var xList = new List<double>(this.Values.SelectMany(pair => pair.Item1));
            return xList;
        }

        public List<double> GetComputeConcatenatedYList()
        {
            var xList = new List<double>(this.Values.SelectMany(pair => pair.Item2));
            return xList;
        }

        public List<double> GetComputeConcatenatedUniqueXList()
        {
            var hashSet = new HashSet<double>(this.Values.SelectMany(pair => pair.Item1));
            return hashSet.ToList();
        }

        public void InterpolateY()
        {
            List<double> commonXList = GetComputeConcatenatedUniqueXList();

            foreach (string key in this.Keys.ToList())
            {
                Tuple<List<double>, List<double>> tuple = this[key];

                List<double> xList = tuple.Item1;
                List<double> yList = tuple.Item2;

                List<double> yInterpolatedList = Interpolate(commonXList, xList, yList);

                this[key] = new Tuple<List<double>, List<double>>(commonXList, yInterpolatedList);
            }
        }

        public List<double> GetComputeMeanY()
        {
            List<double> commonXList = GetComputeConcatenatedUniqueXList();

            if (!commonXList.Any())
            {
                throw new InvalidOperationException("CommonXList is not initialized or empty.");
            }

            return commonXList.Select((x, i) => this.Values.Average(pair => pair.Item2[i])).ToList();
        }

        public List<double> GetComputeStdDevY()
        {
            List<double> meanYList = GetComputeMeanY();

            return GetComputeConcatenatedUniqueXList().Select((x, i) =>
            {
                var deviations = this.Values.Select(pair => pair.Item2[i] - meanYList[i]);
                double variance = deviations.Sum(dev => dev * dev) / deviations.Count();
                return Math.Sqrt(variance);
            }).ToList();
        }

        private List<double> Interpolate(List<double> commonXList, List<double> xList, List<double> yList)
        {
            return MathUtils.Interp(commonXList, xList, yList);
        }

        public string GetConcatenatedKeys()
        {
            return string.Join("", this.Keys);
        }

        public void Concatenate(ListPairs secondDict)
        {
            foreach (var entry in secondDict)
            {
                string key = entry.Key;
                Tuple<List<double>, List<double>> value = entry.Value;

                // Check if the key already exists in the current dictionary
                if (this.ContainsKey(key))
                {
                    // If the key exists, you could decide how to handle it.
                    // For example, you could throw an exception, ignore the entry, merge the lists, or append a suffix to the key.
                    // The following line simply ignores the new entry if the key already exists:
                    continue;
                    // To throw an exception, uncomment the following line:
                    //throw new ArgumentException($"An entry with the same key '{key}' already exists in the dictionary.");
                }
                else
                {
                    // If the key doesn't exist, add the new entry to the current dictionary
                    this.Add(key, value.Item1, value.Item2);
                }
            }
        }

        public void TruncateLists(int length)
        {
            foreach (var key in this.Keys.ToList())
            {
                Tuple<List<double>, List<double>> tuple = this[key];

                List<double> truncatedList1 = tuple.Item1.Take(length).ToList();
                List<double> truncatedList2 = tuple.Item2.Take(length).ToList();

                if (truncatedList1.Count != truncatedList2.Count)
                {
                    throw new InvalidOperationException("Truncated lists have differing lengths, which should never happen.");
                }

                this[key] = new Tuple<List<double>, List<double>>(truncatedList1, truncatedList2);
            }
        }
    }
}


