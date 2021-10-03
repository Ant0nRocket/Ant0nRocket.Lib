using System;

namespace Ant0nRocket.Lib.Std20.DependencyInjection
{
    public class InitializerMethodAttribute : Attribute
    {
        public string InitializerMethodName { get; set; }

        public InitializerMethodAttribute(string methodName = default)
        {
            InitializerMethodName = methodName;
        }
    }
}
