using System.ComponentModel;

namespace Ant0nRocket.Lib.Std20.Tests.Enums
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
