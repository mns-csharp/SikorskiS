using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public static class FileWriter
    {
        public static void WriteToFile(string dirPath, string fileName, string text)
        {
            // Ensure that the directory exists
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Combine the directory path and file name
            string filePath = Path.Combine(dirPath, fileName);

            // Append the text to the file, creating it if it doesn't exist
            File.AppendAllText(filePath, text + "\n");
        }

        public static void WriteToFile(string dirPath, string fileName, List<double> xList)
        {
            if (xList.Count <=0)
            {
                throw new DataMisalignedException("xList must be of same size");
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = Path.Combine(dirPath, fileName);

            // Create a new StreamWriter instance, or open the existing file for appending
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                // Iterate through the lists and write the elements side-by-side
                for (int i = 0; i < xList.Count ; i++)
                {
                    sw.WriteLine($"{xList[i]}");
                }
            }
        }

        public static void WriteToFile(string dirPath, string fileName, List<double> xList, List<double> yList)
        {
            if (xList.Count != yList.Count)
            {
                throw new DataMisalignedException("xList and yList must be of same size");
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            string filePath = Path.Combine(dirPath, fileName);

            // Create a new StreamWriter instance, or open the existing file for appending
            using (StreamWriter sw = new StreamWriter(filePath, append: true))
            {
                // Iterate through the lists and write the elements side-by-side
                for (int i = 0; i < Math.Min(xList.Count, yList.Count); i++)
                {
                    sw.WriteLine($"{xList[i]} {yList[i]}");
                }
            }
        }
    }
}


