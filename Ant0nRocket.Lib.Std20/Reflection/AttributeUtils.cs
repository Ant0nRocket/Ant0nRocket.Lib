using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Std20.Reflection
{
    public static class AttributeUtils
    {
        public static T GetAttribute<T>(Type type) where T : Attribute =>
            (T)Attribute.GetCustomAttribute(type, typeof(T));

        public static Type GetTypeAccrossAppDomain(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                var targetType = types.Where(t => t.FullName == typeName).FirstOrDefault();
                if (targetType != null)
                    return targetType;
            }

            return null;
        }

        public static List<Type> GetTypesAccrossAppDomainWithAttribute<T>() where T : Attribute
        {
            var result = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var targetAttribute = GetAttribute<T>(type);
                    if (targetAttribute != default)
                        result.Add(type);
                }
            }

            return result;
        }
    }    
}
