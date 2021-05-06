using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace JOS.FlatDictionary
{
    public class Implementation2 : IFlatDictionaryProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachedTypeProperties;

        static Implementation2()
        {
            CachedTypeProperties = new ConcurrentDictionary<Type, PropertyInfo[]>();
        }

        public Dictionary<string, string> Execute(object @object, string prefix = "")
        {
            var dictionary = new Dictionary<string, string>();
            Flatten(dictionary, @object, prefix);
            return dictionary;
        }

        private static void Flatten(
            IDictionary<string, string> dictionary,
            object source,
            string name)
        {
            var properties = GetProperties(source.GetType());
            foreach (var property in properties)
            {
                var key = string.IsNullOrWhiteSpace(name) ? property.Name : $"{name}.{property.Name}";
                var value = property.GetValue(source, null);

                if (value == null)
                {
                    dictionary[key] = null;
                    continue;
                }

                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    if (value is DateTime dateTime)
                    {
                        dictionary[key] = dateTime.ToString("o");
                    }
                    else if (value is bool @bool)
                    {
                        dictionary[key] = @bool.ToString().ToLower();
                    }
                    else
                    {
                        dictionary[key] = value.ToString();
                    }
                }
                else if (value is IEnumerable enumerable)
                {
                    var counter = 0;
                    foreach (var item in enumerable)
                    {
                        var itemKey = $"{key}[{counter++}]";
                        if (item.GetType().IsReferenceType())
                        {
                            Flatten(dictionary, item, itemKey);
                        }
                        else
                        {
                            dictionary.Add(itemKey, item.FormatValue());
                        }
                    }
                }
                else
                {
                    Flatten(dictionary, value, key);
                }
            }
        }

        private static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if (CachedTypeProperties.TryGetValue(type, out var result))
            {
                return result;
            }

            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            CachedTypeProperties.TryAdd(type, properties);
            return properties;
        }
    }
}
