using System;
using System.Collections.Generic;
using System.IO;

namespace Figure_7_Sikorski
{
    public static class Settings
    {
        private static Dictionary<SettingsOptions, string> SettingsLines_;
        public static string SettingsFile { get; set; } = "settings.txt";

        static Settings()
        {
            SettingsLines_ = Settings.GetSetting();
        }

        public static string InputDirectory
        {
            get
            {
                return SettingsLines_[SettingsOptions.DataPath];
            }
        }

        // Property to get the solution directory path
        public static string OutputDirectory
        {
            get
            {
                return SettingsLines_[SettingsOptions.OutputPath];
            }
        }

        public static bool IsConvertYtoLogY
        {
            get
            {
                return Convert.ToBoolean(SettingsLines_[SettingsOptions.ConvertYdataToLogY]);
            }
        }

        public static double AutoCorrThresholdMin
        {
            get
            {
                return Convert.ToDouble(SettingsLines_[SettingsOptions.AutoCorrelationMin]);
            }
        }

        public static double AutoCorrThresholdMax
        {
            get
            {
                return Convert.ToDouble(SettingsLines_[SettingsOptions.AutoCorrelationMax]);
            }
        }

        public static int NumAutoCorrLags
        {
            get
            {
                return Convert.ToInt32(SettingsLines_[SettingsOptions.NumberOfLags]);
            }
        }

        public static bool NormAutoCorr
        {
            get
            {
                return Convert.ToBoolean(SettingsLines_[SettingsOptions.NormalizeAutoCorr]);
            }
        }

        public static bool Parallelize
        {
            get
            {
                return Convert.ToBoolean(SettingsLines_[SettingsOptions.Parallelize]);
            }
        }

        public static DataFileTypeEnum DataFileType
        {
            get
            {
                return DataFileTypeEnumConverter.StringToEnum(SettingsLines_[SettingsOptions.DataFileName]);
            }
        }

        public static string DataFileName
        {
            get
            {
                return DataFileTypeEnumConverter.EnumToString(Settings.DataFileType);
            }
        }

        public static string SimulationID
        {
            get
            {
                return SettingsLines_[SettingsOptions.SimulationID];
            }
        }

        public static Dictionary<SettingsOptions, string> GetSetting()
        {
            // Let's assume that the settings content is read from a file named "settings.txt"
            string[] settingsLines = File.ReadAllLines(SettingsFile);
            var settings = ParseSettings(settingsLines);
            return settings;
        }

        private static Dictionary<SettingsOptions, string> ParseSettings(string[] settingsLines)
        {
            var settingsDictionary = new Dictionary<SettingsOptions, string>();

            foreach (var line in settingsLines)
            {
                if (!line.StartsWith("#") && line.Contains("="))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length == 2)
                    {
                        SettingsOptions option;
                        if (Enum.TryParse(parts[0].Trim(), out option))
                        {
                            settingsDictionary[option] = parts[1].Trim();
                        }
                    }
                }
            }

            return settingsDictionary;
        }
    }
}
