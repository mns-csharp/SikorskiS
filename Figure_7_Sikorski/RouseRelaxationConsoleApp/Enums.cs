using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figure_7_Sikorski
{
    public enum DataFileTypeEnum
    {
        [Description("r2.dat")]
        R2,

        [Description("r_end_vec.dat")]
        RendVec
    }

    public enum SettingsOptions
    {
        DataPath,
        OutputPath,
        ConvertYdataToLogY,
        AutoCorrelationMin,
        AutoCorrelationMax,
        NumberOfLags,
        NormalizeAutoCorr,
        DataFileName,
        SimulationID,
        Parallelize
    }
}
