using BenchmarkDotNet.Running;

namespace Ant0nRocket.Lib.Benchmarks
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<Extensions_Benchmarks>();
        }
    }
}
