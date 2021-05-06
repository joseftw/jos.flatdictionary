using System;
using System.Collections.Generic;

namespace JOS.FlatDictionary
{
    public class MyClass
    {
        public bool Boolean { get; set; }
        public string String { get; set; }
        public Guid Guid { get; set; }
        public int Integer { get; set; }
        public IEnumerable<MyClass> MyClasses { get; set; }
        public IEnumerable<string> Strings { get; set; }
        public MyNestedClass MyNestedClass { get; set; }
    }

    public class MyNestedClass
    {
        public bool Boolean { get; set; }
        public string String { get; set; }
        public Guid Guid { get; set; }
        public int Integer { get; set; }
    }
}
