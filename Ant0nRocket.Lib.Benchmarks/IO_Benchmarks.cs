using Ant0nRocket.Lib.IO;
using Ant0nRocket.Lib.IO.FileSystem;

using BenchmarkDotNet.Attributes;

using OneOf.Types;

namespace Ant0nRocket.Lib.Benchmarks
{
    [MemoryDiagnoser]
    [WarmupCount(0)]
    public class IO_Benchmarks
    {
        //[Benchmark]
        public void TouchDirectory()
        {
            for (var i = 0; i < 1_000; i++)
            {
                _ = FileSystem.TouchDirectory("|");
            }
        }

        //[Benchmark]
        public void AlocationTest()
        {
            var l = new List<Success>(1_000_000);
            for (var i = 0; i < l.Capacity; i++)
                l.Add(new Success());
        }

        //[Benchmark]
        public void TouchDirectoryOld()
        {
            for (var i = 0; i < 1_000; i++)
            {
                _ = FileSystemUtils.TouchDirectory("|");
            }
        }

        [Benchmark]
        public void TouchDirectoryUnauthorized()
        {
            for (var i = 0; i < 1_000; i++)
            {
                _ = FileSystem.TouchDirectory(@"C:\Windows\_Test");
            }
        }

        [Benchmark]
        public void TouchDirectoryOldUnauthorized()
        {
            for (var i = 0; i < 1_000; i++)
            {
                _ = FileSystem.TouchDirectory(@"C:\Windows\_Test");
            }
        }
    }
}
