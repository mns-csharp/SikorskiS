using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Figure_7_Sikorski
{
    public class FileFilter
    {
        private readonly string baseDirectory_;
        private readonly string regExPattern_;

        public FileFilter(string baseDirectory, string regExPattern)
        {
            baseDirectory_ = baseDirectory;
            regExPattern_ = regExPattern;
        }

        public List<string> GetMatchingFiles()
        {
            // Ensure the base directory exists
            if (!Directory.Exists(baseDirectory_))
            {
                throw new DirectoryNotFoundException($"The directory {baseDirectory_} does not exist.");
            }

            // Compile the regular expression pattern
            Regex regex = new Regex(regExPattern_);

            // Get all files in the directory, you may want to search in subdirectories as well
            // If so, use SearchOption.AllDirectories instead of SearchOption.TopDirectoryOnly
            var files = Directory.EnumerateFiles(baseDirectory_, "*", SearchOption.AllDirectories);

            // Filter the files using the regex pattern
            var matchingFiles = files.Where(file => regex.IsMatch(Path.GetFileName(file))).ToList();

            return matchingFiles;
        }
    }
}

