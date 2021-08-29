using System;
using BenchmarkDotNet.Running;

namespace Helix.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<GuildServiceBenchmarks>();
            Console.ReadLine();
        }
    }
}
