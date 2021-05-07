using System;
using System.Collections.Generic;

namespace JOS.FlatDictionary
{
    public class HardCodedImplementation : IFlatDictionaryProvider
    {
        public Dictionary<string, string> Execute(object @object, string prefix = "")
        {
            var myClass = (MyClass)@object;
            var dictionary = new Dictionary<string, string>
            {
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.Boolean)}" , myClass.Boolean.ToStringLowerCase()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.Integer)}", myClass.Integer.ToString()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.Guid)}", myClass.Guid.ToString()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.String)}", myClass.String},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyNestedClass)}.{nameof(myClass.MyNestedClass.Boolean)}", myClass.MyNestedClass.Boolean.ToStringLowerCase()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyNestedClass)}.{nameof(myClass.MyNestedClass.Integer)}", myClass.MyNestedClass.Integer.ToString()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyNestedClass)}.{nameof(myClass.MyNestedClass.Guid)}", myClass.MyNestedClass.Guid.ToString()},
                { $"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyNestedClass)}.{nameof(myClass.MyNestedClass.String)}", myClass.MyNestedClass.String}
            };

            var counter = 0;

            foreach (var @string in myClass?.Strings ?? Array.Empty<string>())
            {
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.Strings)}[{counter++}]", @string);
            }

            counter = 0;
            foreach (var myClassItem in myClass.MyClasses)
            {
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.Boolean)}", myClassItem.Boolean.ToStringLowerCase());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.Integer)}", myClassItem.Integer.ToString());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.Guid)}", myClassItem.Guid.ToString());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.String)}", myClassItem.String);
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.MyNestedClass)}.{nameof(myClassItem.MyNestedClass.Boolean)}", myClassItem.MyNestedClass.Boolean.ToStringLowerCase());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.MyNestedClass)}.{nameof(myClassItem.MyNestedClass.Integer)}", myClassItem.MyNestedClass.Integer.ToString());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.MyNestedClass)}.{nameof(myClassItem.MyNestedClass.Guid)}", myClassItem.MyNestedClass.Guid.ToString());
                dictionary.Add($"{(string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"{prefix}.")}{nameof(myClass.MyClasses)}[{counter}].{nameof(myClassItem.MyNestedClass)}.{nameof(myClassItem.MyNestedClass.String)}", myClassItem.MyNestedClass.String);
                counter++;
            }

            return dictionary;
        }
    }
}
