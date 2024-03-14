using Ant0nRocket.Lib.Extensions;
using BenchmarkDotNet.Attributes;

namespace Ant0nRocket.Lib.Benchmarks
{
    [MemoryDiagnoser]
    public class Extensions_Benchmarks
    {
        [Benchmark]
        public void ByteArrayExtensions_ComputeHash()
        {
            var bytesArray = Array.Empty<byte>();
            _ = bytesArray.ComputeHash();
        }
    }
}
