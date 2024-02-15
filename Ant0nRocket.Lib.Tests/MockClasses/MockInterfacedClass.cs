using Ant0nRocket.Lib.Tests.MockInterfaces;

namespace Ant0nRocket.Lib.Tests.MockClasses
{
    public class MockInterfacedClass : IMockInterface
    {
        public int SomeInt { get; set; } = 10;
    }
}
