using System;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Std20.Reflection
{
    /// <summary>
    /// Collection of reflection utils.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Performes search of type full name specified as string 
        /// <paramref name="typeName"/> and returnes <see cref="Type"/> if
        /// found one in current domain. If not found - <b>null</b>.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type FindTypeAccrossAppDomain(string typeName)
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

        /// <summary>
        /// Performes search of all classes (and only classes!) that implements
        /// specified by <typeparamref name="T"/> interface.<br />
        /// If any class found it will be added to result list, or empty list returned.
        /// </summary>
        public static IEnumerable<Type> GetClassesThatImplementsInterface<T>() where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"Type '{typeof(T).Name}' is not an interface");

            var resultList = new List<Type>();
            var assemblies = AppDomain
                .CurrentDomain
                .GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetTypes()
                    .Where(t => t.IsClass);

                foreach (var type in types)
                {
                    if (typeof(T).IsAssignableFrom(type))
                        resultList.Add(type);
                }
            }

            return resultList;
        }
    }
}
