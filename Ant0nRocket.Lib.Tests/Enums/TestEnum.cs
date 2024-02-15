using System.ComponentModel;

namespace Ant0nRocket.Lib.Tests.Enums
{
    internal enum TestEnum
    {
        [Description("First")]
        SomeFirstValue,

        [Description("Second")]
        SomeSecondValue,

        // No description
        SomeThirdValue,
    }
}
