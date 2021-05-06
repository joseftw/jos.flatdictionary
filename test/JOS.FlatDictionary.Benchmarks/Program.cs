using BenchmarkDotNet.Running;

namespace JOS.FlatDictionary.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary1 = BenchmarkRunner.Run<FlatDictionaryBenchmark>();
        }
    }
}
