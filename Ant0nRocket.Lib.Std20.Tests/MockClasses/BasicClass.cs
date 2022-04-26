using System;

using Ant0nRocket.Lib.Std20.Tests.MockAttributes;

namespace Ant0nRocket.Lib.Std20.Tests.MockClasses
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
