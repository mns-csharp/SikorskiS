using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Figure_7_Sikorski
{
    public class DirectoryFilter
    {
        private readonly string baseDirectory_;
        private readonly string pattern_;

        public DirectoryFilter(string baseDirectory, string pattern)
        {
            baseDirectory_ = baseDirectory;
            pattern_ = pattern;
        }

        public List<string> GetMatchingDirectories()
        {
            if (!Directory.Exists(baseDirectory_))
            {
                throw new DirectoryNotFoundException($"The directory '{baseDirectory_}' does not exist.");
            }

            Regex dirPattern = new Regex(pattern_, RegexOptions.IgnoreCase);

            return Directory.EnumerateDirectories(baseDirectory_)
                .Where(dir => dirPattern.IsMatch(Path.GetFileName(dir)))
                .ToList();
        }
    }
}