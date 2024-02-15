using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Tests.MockAttributes;

namespace Ant0nRocket.Lib.Tests.MockClasses
{
    [Store("StoreClass")]
    [SomeCustom]
    public class StoreClass
    {
        public string TestString { get; set; } = "Hello";
    }
}
