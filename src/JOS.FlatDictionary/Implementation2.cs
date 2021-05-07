using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

                if (property.PropertyType.IsValueTypeOrString())
                {
                    dictionary[key] = value switch
                    {
                        DateTime dateTime => dateTime.ToString("o"),
                        bool @bool => @bool.ToString().ToLower(),
                        _ => value.ToString()
                    };
                }
                else if (value is IEnumerable enumerable)
                {
                    var counter = 0;
                    foreach (var item in enumerable)
                    {
                        var itemKey = $"{key}[{counter++}]";
                        if (!item.GetType().IsValueTypeOrString())
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

            var properties = type.GetProperties().Where(x => x.CanRead).ToArray();
            CachedTypeProperties.TryAdd(type, properties);
            return properties;
        }
    }
}
