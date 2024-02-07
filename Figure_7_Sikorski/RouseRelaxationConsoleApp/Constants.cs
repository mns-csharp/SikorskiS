using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{    
    public static class Constants
    {
        public static string MultipleRunPattern = @"^\d{8}_\d{6}$";
        public static string SingleRunDirPattern = @"run\d+_inner\d+_outer\d+_factor\d+_residue\d+";

        public static string CurrentTime
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMdd_HH_mm_ss");
            }
        }
    }
}
