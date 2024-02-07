using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Figure_7_Sikorski
{
    public static class FileReader
    {
        public static List<string> ReadLines(string filePath)
        {
            string text = ReadText(filePath);
            return text?.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                        .Where(line => !line.StartsWith("#"))
                        .ToList() ?? new List<string>();
        }

        public static string ReadText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return string.Empty;
            }
        }

        public static string ReadOneLine(string filePath)
        {
            try
            {
                var reader = new StreamReader(filePath);
                return reader.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return string.Empty;
            }
        }

        public static List<double> ReadOneColumn(string inputPath, string datFileName)
        {
            string filePath = Path.Combine(inputPath, datFileName);

            List<double> data = new List<double>();
            foreach (var line in File.ReadLines(filePath))
            {
                if (double.TryParse(line, out double value))
                {
                    data.Add(value);
                }
            }
            return data;
        }

        public static (List<double>, List<double>) ReadTwoColumns(string dirPath, string fileName)
        {
            var filePath = Path.Combine(dirPath, fileName);
            var column1 = new List<double>();
            var column2 = new List<double>();

            foreach (var line in ReadLinesWithSkipEmpty(filePath))
            {
                var split = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length >= 2)
                {
                    if (double.TryParse(split[0], out double firstValue))
                    {
                        column1.Add(firstValue);
                    }

                    if (double.TryParse(split[1], out double secondValue))
                    {
                        column2.Add(secondValue);
                    }
                }
            }

            return (column1, column2);
        }

        public static (List<double>, List<double>, List<double>) ReadThreeColumns(string dirPath, string fileName)
        {
            var filePath = Path.Combine(dirPath, fileName);
            var column1 = new List<double>();
            var column2 = new List<double>();
            var column3 = new List<double>();

            foreach (var line in ReadLinesWithSkipEmpty(filePath))
            {
                var split = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length >= 3)
                {
                    if (double.TryParse(split[0], out double firstValue))
                    {
                        column1.Add(firstValue);
                    }

                    if (double.TryParse(split[1], out double secondValue))
                    {
                        column2.Add(secondValue);
                    }

                    if (double.TryParse(split[2], out double thirdValue))
                    {
                        column3.Add(thirdValue);
                    }
                }
            }

            return (column1, column2, column3);
        }

        public static List<Vector3> ReadVec3(string dirPath, string fileName)
        {
            var filePath = Path.Combine(dirPath, fileName);
            var vec3List = new List<Vector3>();

            foreach (var line in ReadLinesWithSkipEmpty(filePath))
            {
                var vec3 = ParseVec3(line);
                if (vec3 != null)
                {
                    vec3List.Add(vec3);
                }
            }

            return vec3List;
        }

        public static List<double> ReadVec3Magnitude(string dirPath, string fileName)
        {
            var filePath = Path.Combine(dirPath, fileName);
            var magnitudes = new List<double>();

            foreach (var line in ReadLinesWithSkipEmpty(filePath))
            {
                var vec3 = ParseVec3(line);

                if (vec3 != null)
                {
                    magnitudes.Add(vec3.Magnitude());
                }
            }

            return magnitudes;
        }

        private static Vector3 ParseVec3(string line)
        {
            var split = line.Split(new[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length >= 3 &&
                double.TryParse(split[0], out double x) &&
                double.TryParse(split[1], out double y) &&
                double.TryParse(split[2], out double z))
            {
                return new Vector3(x, y, z);
            }

            return null;
        }

        private static IEnumerable<string> ReadLinesWithSkipEmpty(string filePath)
        {
            return File.ReadLines(filePath).Where(l => !string.IsNullOrWhiteSpace(l));
        }
    }
}







