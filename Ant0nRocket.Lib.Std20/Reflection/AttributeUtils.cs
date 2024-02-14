using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Std20.Reflection
{
    public static class AttributeUtils
    {
        [Obsolete("Use ReflectionUtils.GetAttribute")]
        public static T GetAttribute<T>(Type type) where T : Attribute =>
            (T)Attribute.GetCustomAttribute(type, typeof(T));

        [Obsolete]
        public static List<Type> GetTypesAccrossAppDomainWithAttribute<T>() where T : Attribute
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var targetAttribute = ReflectionUtils.GetAttribute<T>(type);
                    if (targetAttribute != default)
                        result.Add(type);
                }
            }

            return result;
        }
    }    
}
