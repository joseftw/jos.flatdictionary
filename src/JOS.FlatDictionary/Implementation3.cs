using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JOS.FlatDictionary
{
    public class Implementation3 : IFlatDictionaryProvider
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>> CachedProperties;

        static Implementation3()
        {
            CachedProperties = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>>();
        }

        public Dictionary<string, string> Execute(object @object, string prefix = "")
        {
            return ExecuteInternal(@object, prefix: prefix);
        }

        private static Dictionary<string, string> ExecuteInternal(
            object @object,
            Dictionary<string, string> dictionary = default,
            string prefix = "")
        {
            dictionary ??= new Dictionary<string, string>();
            var type = @object.GetType();
            var properties = GetProperties(type);

            foreach (var (property, getter) in properties)
            {
                var key = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
                var value = getter(@object);

                if (value == null)
                {
                    dictionary.Add(key, null);
                    continue;
                }

                if (!property.PropertyType.IsValueTypeOrString())
                {
                    if (value is IEnumerable enumerable)
                    {
                        var counter = 0;
                        foreach (var item in enumerable)
                        {
                            var itemKey = $"{key}[{counter++}]";
                            var itemType = item.GetType();
                            if (!itemType.IsValueTypeOrString())
                            {
                                ExecuteInternal(item, dictionary, itemKey);
                            }
                            else
                            {
                                dictionary.Add(itemKey, item.FormatValue());
                            }
                        }
                    }
                    else
                    {
                        ExecuteInternal(value, dictionary, key);
                    }
                }
                else
                {
                    dictionary.Add(key, value.FormatValue());
                }
            }

            return dictionary;
        }

        private static Dictionary<PropertyInfo, Func<object, object>> GetProperties(Type type)
        {
            if (CachedProperties.TryGetValue(type, out var properties))
            {
                return properties;
            }

            CacheProperties(type);
            return CachedProperties[type];
        }
        
        private static void CacheProperties(Type type)
        {
            if (CachedProperties.ContainsKey(type))
            {
                return;
            }

            CachedProperties[type] = new Dictionary<PropertyInfo, Func<object, object>>();
            var properties = type.GetProperties().Where(x => x.CanRead);
            foreach (var propertyInfo in properties)
            {
                var getter = CompilePropertyGetter(propertyInfo);
                CachedProperties[type].Add(propertyInfo, getter);
                if (propertyInfo.PropertyType.IsValueTypeOrString())
                {
                    if (propertyInfo.PropertyType.IsIEnumerable())
                    {
                        var types = propertyInfo.PropertyType.GetGenericArguments();
                        foreach (var genericType in types)
                        {
                            if (!genericType.IsValueTypeOrString())
                            {
                                CacheProperties(genericType);
                            }
                        }
                    }
                    else
                    {
                        CacheProperties(propertyInfo.PropertyType);
                    }
                }
            }
        }

        private static Func<object, object> CompilePropertyGetter(PropertyInfo property)
        {
            var objectParameter = Expression.Parameter(typeof(object));
            var typeAsExpression = Expression.TypeAs(objectParameter, property.DeclaringType);
            var propertyExpression = Expression.Property(typeAsExpression, property);
            var convertExpression = Expression.Convert(propertyExpression, typeof(object));
            return Expression.Lambda<Func<object, object>>(
                convertExpression,
                objectParameter).Compile();
        }
    }
}
