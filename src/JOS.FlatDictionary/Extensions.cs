using System;
using System.Collections;

namespace JOS.FlatDictionary
{
    internal static class Extensions
    {
        internal static bool IsValueTypeOrString(this Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        internal static string ToStringValueType(this object value)
        {
            return value switch
            {
                DateTime dateTime => dateTime.ToString("o"),
                bool boolean => boolean.ToStringLowerCase(),
                _ => value.ToString()
            };
        }

        internal static bool IsIEnumerable(this Type type)
        {
            return type.IsAssignableTo(typeof(IEnumerable));
        }

        internal static string ToStringLowerCase(this bool boolean)
        {
            return boolean ? "true" : "false";
        }
    }
}
