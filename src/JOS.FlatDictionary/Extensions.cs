using System;

namespace JOS.FlatDictionary
{
    public static class Extensions
    {
        public static bool IsReferenceType(this Type type)
        {
            return !type.IsValueType && type != typeof(string);
        }

        public static string FormatValue(this object value)
        {
            return value switch
            {
                DateTime dateTime => dateTime.ToString("o"),
                bool boolean => boolean.ToString().ToLower(),
                _ => value.ToString()
            };
        }
    }
}
