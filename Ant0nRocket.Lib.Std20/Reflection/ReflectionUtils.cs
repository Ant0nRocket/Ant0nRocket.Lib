using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Ant0nRocket.Lib.Std20.Reflection
{
    /// <summary>
    /// Collection of reflection utils.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Name of the application
        /// </summary>
        private static string? __appName = default;

        /// <summary>
        /// Allows you to set app name manually.
        /// If you never set the app name - reflections will get it from Assembly
        /// </summary>
        public static void SetAppName(string appName)
        {
            if (!string.IsNullOrEmpty(appName) && !string.IsNullOrWhiteSpace(appName))
                __appName = appName;
        }

        /// <summary>
        /// Returnes value specified by <see cref="SetAppName(string)"/>
        /// of <see cref="Assembly.GetEntryAssembly"/> name.
        /// </summary>
        public static string GetAppName()
        {
            if (__appName != default)
                return __appName;

            return Assembly.GetEntryAssembly()?.GetName()?.Name!;
        }

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
                return (T)Attribute.GetCustomAttribute(fromType, typeof(T));
            }
            catch (Exception ex)
            {
                SignalBus.Send(ex);
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

        private static readonly Logger _logger = Logger.Create(nameof(ReflectionUtils));

        //private static string? _appName = default;

        /// <summary>
        /// Leave it default if you want an AppName from assembly name.
        /// </summary>
        [Obsolete]
        public static string AppName { get => GetAppName(); set => SetAppName(value); }

        /// <summary>
        /// Performes search of type full name specified as string 
        /// <paramref name="typeName"/> and returnes <see cref="Type"/> if
        /// found one in current domain. If not found - <b>null</b>.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        [Obsolete]
        public static Type? FindTypeAccrossAppDomain(string typeName)
        {
            return FindType(typeName);
        }

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



            //var resultList = new List<Type>();
            //var assemblies = AppDomain
            //    .CurrentDomain
            //    .GetAssemblies();

            //foreach (var assembly in assemblies)
            //{
            //    var types = assembly
            //        .GetTypes()
            //        .Where(t => t.IsClass && !t.IsAbstract);

            //    foreach (var type in types)
            //    {
            //        if (typeof(T).IsAssignableFrom(type))
            //            resultList.Add(type);
            //    }
            //}

            //return resultList;
        }

        #endregion
    }
}
