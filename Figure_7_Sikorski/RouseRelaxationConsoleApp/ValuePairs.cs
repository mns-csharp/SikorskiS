using System;
using System.Collections.Generic;
using System.Linq;

namespace Figure_7_Sikorski
{
    public class ValuePairs : Dictionary<string, Tuple<double, double>>
    {
        public void Add(string key, double list1, double list2)
        {
            var tuple = new Tuple<double, double>(list1, list2);
            this[key] = tuple;
        }

        public List<double> ComputeConcatenatedXListUnique()
        {
            var hashSet = new HashSet<double>(this.Values.Select(tuple => tuple.Item1));
            return hashSet.ToList();
        }

        public List<double> ComputeConcatenatedYListUnique()
        {
            var hashSet = new HashSet<double>(this.Values.Select(tuple => tuple.Item2));
            return hashSet.ToList();
        }

        public List<double> ComputeMeanY()
        {
            List<double> commonXList = ComputeConcatenatedXListUnique();

            if (!commonXList.Any())
            {
                throw new InvalidOperationException("CommonXList is not initialized or empty.");
            }

            return commonXList.Select(x => this.Values.Where(tuple => tuple.Item1.Equals(x)).Average(tuple => tuple.Item2)).ToList();
        }

        public List<double> ComputeStdDevY()
        {
            List<double> meanYList = ComputeMeanY();
            List<double> commonXList = ComputeConcatenatedXListUnique();

            return commonXList.Select(x =>
            {
                var yValues = this.Values.Where(tuple => tuple.Item1.Equals(x)).Select(tuple => tuple.Item2).ToList();
                double meanY = yValues.Average();
                double variance = yValues.Sum(y => Math.Pow(y - meanY, 2)) / yValues.Count;
                return Math.Sqrt(variance);
            }).ToList();
        }

        public string GetConcatenatedKeys()
        {
            return string.Join("", this.Keys);
        }

        public void Concatenate(ValuePairs secondDict)
        {
            foreach (var entry in secondDict)
            {
                string key = entry.Key;
                Tuple<double, double> value = entry.Value;

                if (!this.ContainsKey(key))
                {
                    this.Add(key, value.Item1, value.Item2);
                }
            }
        }
    }
}