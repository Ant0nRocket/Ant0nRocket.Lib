using System;
using Ant0nRocket.Lib.Tests.MockAttributes;

namespace Ant0nRocket.Lib.Tests.MockClasses
{
    [SomeCustom]
    public class BasicClass
    {
        public BasicClass()
        {
            SomeInt = new Random(DateTime.Now.Millisecond).Next();
        }

        public int SomeInt { get; set; }
    }
}
