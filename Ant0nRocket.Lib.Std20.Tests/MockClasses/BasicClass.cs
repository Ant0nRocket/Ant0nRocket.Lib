using System;

namespace Ant0nRocket.Lib.Std20.Tests.MockClasses
{
    public class BasicClass
    {
        public BasicClass()
        {
            SomeInt = new Random(DateTime.Now.Millisecond).Next();
        }

        public int SomeInt { get; set; }
    }
}
