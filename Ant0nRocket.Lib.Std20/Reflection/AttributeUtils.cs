using System;

namespace Ant0nRocket.Lib.Std20.Reflection
{
    public static class AttributeUtils
    {
        public static T GetAttribute<T>(Type type) where T : Attribute =>
            (T)Attribute.GetCustomAttribute(type, typeof(T));
    }
}
