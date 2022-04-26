using Ant0nRocket.Lib.Std20.Attributes;
using Ant0nRocket.Lib.Std20.Tests.MockAttributes;

namespace Ant0nRocket.Lib.Std20.Tests.MockClasses
{
    [Store("StoreClass")]
    [SomeCustom]
    public class StoreClass
    {
        public string TestString { get; set; } = "Hello";
    }
}
