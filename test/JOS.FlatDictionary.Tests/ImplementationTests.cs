using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace JOS.FlatDictionary.Tests
{
    public class ImplementationTests
    {
        private readonly MyClass MyClass;

        public ImplementationTests()
        {
            MyClass = new MyClass
            {
                Boolean = true,
                Guid = Guid.NewGuid(),
                Integer = 100,
                String = "string",
                MyNestedClass = new MyNestedClass
                {
                    Boolean = true,
                    Guid = Guid.NewGuid(),
                    Integer = 100,
                    String = "string"
                },
                MyClasses = new List<MyClass>
                    {
                        new MyClass
                        {
                            Boolean = true,
                            Guid = Guid.NewGuid(),
                            Integer = 100,
                            String = "string",
                            MyNestedClass = new MyNestedClass
                            {
                                Boolean = true,
                                Guid = Guid.NewGuid(),
                                Integer = 100,
                                String = "string"
                            }
                        },
                        new MyClass
                        {
                            Boolean = true,
                            Guid = Guid.NewGuid(),
                            Integer = 100,
                            String = "string",
                            MyNestedClass = new MyNestedClass
                            {
                                Boolean = true,
                                Guid = Guid.NewGuid(),
                                Integer = 100,
                                String = "string"
                            }
                        }
                    },
            };
        }

        [Theory]
        [InlineData(typeof(Implementation1))]
        [InlineData(typeof(Implementation2))]
        [InlineData(typeof(Implementation3))]
        [InlineData(typeof(Implementation4HardCoded))]
        public void ShouldHandleValueTypesAndStringsCorrectly(Type implementationType)
        {
            var sut = (IFlatDictionaryProvider)Activator.CreateInstance(implementationType);
            var prefix = "Data";

            var result = sut.Execute(MyClass, prefix: prefix);

            result.ShouldContainKeyAndValue($"{prefix}.Boolean", "true");
            result.ShouldContainKeyAndValue($"{prefix}.Guid", MyClass.Guid.ToString());
            result.ShouldContainKeyAndValue($"{prefix}.Integer", MyClass.Integer.ToString());
            result.ShouldContainKeyAndValue($"{prefix}.String", MyClass.String);
        }

        [Theory]
        [InlineData(typeof(Implementation1))]
        [InlineData(typeof(Implementation2))]
        [InlineData(typeof(Implementation3))]
        [InlineData(typeof(Implementation4HardCoded))]
        public void ShouldHandleReferenceTypeProperties(Type implementationType)
        {
            var sut = (IFlatDictionaryProvider)Activator.CreateInstance(implementationType);
            var prefix = "Data";

            var result = sut.Execute(MyClass, prefix: prefix);

            result.ShouldContainKeyAndValue("Data.MyNestedClass.Boolean", MyClass.MyNestedClass.Boolean.ToString().ToLower());
            result.ShouldContainKeyAndValue("Data.MyNestedClass.Guid", MyClass.MyNestedClass.Guid.ToString());
            result.ShouldContainKeyAndValue("Data.MyNestedClass.Integer", MyClass.MyNestedClass.Integer.ToString());
            result.ShouldContainKeyAndValue("Data.MyNestedClass.String", MyClass.MyNestedClass.String);
        }

        [Theory]
        [InlineData(typeof(Implementation1))]
        [InlineData(typeof(Implementation2))]
        [InlineData(typeof(Implementation3))]
        [InlineData(typeof(Implementation4HardCoded))]
        public void ShouldHandleIEnumerableStringProperties(Type implementationType)
        {
            var sut = (IFlatDictionaryProvider)Activator.CreateInstance(implementationType);
            var prefix = "Data";
            MyClass.Strings = new List<string> { "string1", "string2" };

            var result = sut.Execute(MyClass, prefix: prefix);
            var stringsList = MyClass.Strings.ToList();

            result.ShouldContainKeyAndValue("Data.Strings[0]", stringsList[0]);
            result.ShouldContainKeyAndValue("Data.Strings[1]", stringsList[1]);
        }

        [Theory]
        [InlineData(typeof(Implementation1))]
        [InlineData(typeof(Implementation2))]
        [InlineData(typeof(Implementation3))]
        [InlineData(typeof(Implementation4HardCoded))]
        public void ShouldHandleIEnumerableReferenceProperties(Type implementationType)
        {
            var sut = (IFlatDictionaryProvider)Activator.CreateInstance(implementationType);
            var prefix = "Data";

            var result = sut.Execute(MyClass, prefix: prefix);
            var myClassesList = MyClass.MyClasses.ToList();

            result.ShouldContainKeyAndValue("Data.MyClasses[0].Boolean", myClassesList[0].Boolean.ToString().ToLower());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].Guid", myClassesList[0].Guid.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].Integer", myClassesList[0].Integer.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].String", myClassesList[0].String);
            result.ShouldContainKeyAndValue("Data.MyClasses[1].Boolean", myClassesList[1].Boolean.ToString().ToLower());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].Guid", myClassesList[1].Guid.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].Integer", myClassesList[1].Integer.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].String", myClassesList[1].String);
        }

        [Theory]
        [InlineData(typeof(Implementation1))]
        [InlineData(typeof(Implementation2))]
        [InlineData(typeof(Implementation3))]
        [InlineData(typeof(Implementation4HardCoded))]
        public void ShouldHandleIEnumerableWithNestedReferenceProperties(Type implementationType)
        {
            var sut = (IFlatDictionaryProvider)Activator.CreateInstance(implementationType);
            var prefix = "Data";

            var result = sut.Execute(MyClass, prefix: prefix);
            var myClassesList = MyClass.MyClasses.ToList();

            result.ShouldContainKeyAndValue("Data.MyClasses[0].MyNestedClass.Boolean", myClassesList[0].MyNestedClass.Boolean.ToString().ToLower());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].MyNestedClass.Guid", myClassesList[0].MyNestedClass.Guid.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].MyNestedClass.Integer", myClassesList[0].MyNestedClass.Integer.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[0].MyNestedClass.String", myClassesList[0].MyNestedClass.String);
            result.ShouldContainKeyAndValue("Data.MyClasses[1].MyNestedClass.Boolean", myClassesList[1].MyNestedClass.Boolean.ToString().ToLower());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].MyNestedClass.Guid", myClassesList[1].MyNestedClass.Guid.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].MyNestedClass.Integer", myClassesList[1].MyNestedClass.Integer.ToString());
            result.ShouldContainKeyAndValue("Data.MyClasses[1].MyNestedClass.String", myClassesList[1].MyNestedClass.String);
        }
    }
}
