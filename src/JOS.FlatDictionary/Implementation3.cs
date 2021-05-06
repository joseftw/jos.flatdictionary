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
        private static readonly ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>> CachedGetters;

        static Implementation3()
        {
            CachedGetters = new ConcurrentDictionary<Type, Dictionary<PropertyInfo, Func<object, object>>>();
        }

        public Dictionary<string, string> Execute(object @object, string prefix = "")
        {
            return ExecuteInternal(@object, prefix: prefix);
        }

        public Dictionary<string, string> ExecuteInternal(
            object @object,
            Dictionary<string, string> dictionary = default,
            string prefix = "")
        {
            dictionary ??= new Dictionary<string, string>();
            var type = @object.GetType();
            var items = GetProperties(type);

            foreach (var (property, getter) in items)
            {
                var key = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
                var value = getter(@object);

                if (value == null)
                {
                    dictionary.Add(key, null);
                    continue;
                }

                if (!property.PropertyType.IsReferenceType())
                {
                    dictionary.Add(key, value.FormatValue());
                    continue;
                }

                if (value is IEnumerable enumerable)
                {
                    var counter = 0;
                    foreach (var item in enumerable)
                    {
                        var itemKey = $"{key}[{counter++}]";
                        if (item.GetType().IsReferenceType())
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


            return dictionary;
        }

        private static Dictionary<PropertyInfo, Func<object, object>> GetProperties(Type type)
        {
            if (CachedGetters.TryGetValue(type, out var properties))
            {
                return properties;
            }

            CacheGetters(type);
            return CachedGetters[type];
        }

        private static void CacheGetters(Type type)
        {
            if (CachedGetters.ContainsKey(type))
            {
                return;
            }

            CachedGetters[type] = new Dictionary<PropertyInfo, Func<object, object>>();
            var properties = type.GetProperties().Where(x => x.CanRead);
            foreach (var propertyInfo in properties)
            {
                var getter = CompilePropertyGetter(propertyInfo);
                CachedGetters[type].Add(propertyInfo, getter);
                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsValueType)
                {
                    continue;
                }
                
                if (propertyInfo.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                {
                    var types = propertyInfo.PropertyType.GetGenericArguments();
                    foreach (var genericType in types)
                    {
                        if (!genericType.IsValueType && genericType != typeof(string))
                        {
                            CacheGetters(genericType);
                        }
                    }
                }
                else
                {
                    CacheGetters(propertyInfo.PropertyType);
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
