using System;
using System.ComponentModel;
using System.Reflection;

namespace Figure_7_Sikorski
{    
    public static class DataFileTypeEnumConverter
    {
        public static string EnumToString(DataFileTypeEnum dataFileType)
        {
            // Get the type of the enum
            Type type = dataFileType.GetType();

            // Get the MemberInfo object for the enum value
            MemberInfo[] memberInfo = type.GetMember(dataFileType.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                // Try to get the Description attribute on the enum value
                object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    // Return the description from the Description attribute
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }

            // If there is no Description attribute, return the enum as a string
            return dataFileType.ToString();
        }

        public static DataFileTypeEnum StringToEnum(string description)
        {
            foreach (var field in typeof(DataFileTypeEnum).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (DataFileTypeEnum)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException($"No enum value found for description: {description}", nameof(description));
        }
    }
}
