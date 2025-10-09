using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ant0nRocket.Lib.Reflection
{
    /// <summary>
    /// Collection of reflection utils.
    /// </summary>
    public static class ReflectionUtils
    {
        



        /// <summary>
        /// Performes searching of the type <paramref name="typeFullName"/> in AppDomain and
        /// returnes <see cref="Type"/> if found one.
        /// </summary>
        public static Type? FindType(string typeFullName)
        {
            if (__dictName2Type == default)
            {
                __dictName2Type = new();

                ForEachTypeInDomain(type =>
                {
                    if (type.FullName != null && !__dictName2Type.ContainsKey(type.FullName))
                        __dictName2Type.Add(type.FullName, type);
                });
            }

            if (__dictName2Type.ContainsKey(typeFullName))
                return __dictName2Type[typeFullName];

            return default;
        }

        /// <summary>
        /// Cache for <see cref="FindType(string)"/> function
        /// </summary>
        private static Dictionary<string, Type>? __dictName2Type = default;


        /// <summary>
        /// Returnes a list of types that implements <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<Type> GetTypesThatImplements<T>()
        {
            var result = new List<Type>();
            var t = typeof(T);

            ForEachTypeInDomain(type =>
            {
                if (t.Equals(type) == false && t.IsAssignableFrom(type))
                    result.Add(type);
            });

            return result;
        }

        /// <summary>
        /// Retreives <typeparamref name="T"/> from <paramref name="fromType"/>,
        /// or returnes null if nothing found or any exection thrown
        /// </summary>
        public static T? GetAttribute<T>(Type fromType) where T : Attribute
        {
            try
            {
                return (T?)Attribute.GetCustomAttribute(fromType, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        //-------------- PRIVATE FUNCTION -------------------------------------

        /// <summary>
        /// Helper function that iterates all exported types in domain
        /// and allowes you do <paramref name="doSomeActionWith"/> found types
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ForEachTypeInDomain(Action<Type> doSomeActionWith)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    doSomeActionWith(type);
                }
            }

        }

        #region OBSOLETE code

        /// <summary>
        /// Performes search of all classes (and only classes!) that implements
        /// specified by <typeparamref name="T"/> interface.<br />
        /// If any class found it will be added to result list, or empty list returned.
        /// </summary>
        [Obsolete]
        public static IEnumerable<Type> GetClassesThatImplementsInterface<T>() where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException($"Type '{typeof(T).Name}' is not an interface");

            return GetTypesThatImplements<T>();
        }

        #endregion
    }
}
