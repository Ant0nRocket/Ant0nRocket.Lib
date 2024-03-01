using Ant0nRocket.Lib.IO.FileSystem;
using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant0nRocket.Lib.Benchmarks
{
    public class IO_Benchmarks
    {
        [Benchmark]
        public void TouchDirectory()
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = FileSystem.TouchDirectory("|");
            }
        }
    }
}
