using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace JOS.FlatDictionary.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [HtmlExporter]
    public class FlatDictionaryBenchmark
    {
        private static readonly MyClass Data;
        private static readonly Implementation1 Implementation1;
        private static readonly Implementation2 Implementation2;
        private static readonly Implementation3 Implementation3;
        private static readonly HardCodedImplementation ImplementationHardCoded;

        static FlatDictionaryBenchmark()
        {
            Data = new MyClass
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
                }
            };
            Implementation1 = new Implementation1();
            Implementation2 = new Implementation2();
            Implementation3 = new Implementation3();
            ImplementationHardCoded = new HardCodedImplementation();
        }

        [Benchmark(Baseline = true)]
        public Dictionary<string, string> Implementation1Benchmark()
        {
            return Implementation1.Execute(Data);
        }

        [Benchmark]
        public Dictionary<string, string> Implementation2Benchmark()
        {
            return Implementation2.Execute(Data);
        }

        [Benchmark]
        public Dictionary<string, string> Implementation3Benchmark()
        {
            return Implementation3.Execute(Data);
        }

        [Benchmark]
        public Dictionary<string, string> ImplementationHardCodedBenchmark()
        {
            return ImplementationHardCoded.Execute(Data);
        }
    }
}
