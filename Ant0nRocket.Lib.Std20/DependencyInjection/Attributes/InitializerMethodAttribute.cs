using System;

namespace Ant0nRocket.Lib.Std20.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute class for <see cref="DI"/>.<br />
    /// Set a function name (via <i>nameof([functionName])</i>) and
    /// it will be called during object initialization.
    /// </summary>
    public class InitializerMethodAttribute : Attribute
    {
        public string InitializerMethodName { get; set; }

        public InitializerMethodAttribute(string methodName = default)
        {
            InitializerMethodName = methodName;
        }
    }
}
