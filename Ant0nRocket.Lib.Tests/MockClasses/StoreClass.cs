using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Tests.MockAttributes;

namespace Ant0nRocket.Lib.Tests.MockClasses
{
    [AppDataLocation("StoreClass.json", "Test")]
    [SomeCustom]
    public class StoreClass
    {
        public string TestString { get; set; } = "Hello";
    }
}
