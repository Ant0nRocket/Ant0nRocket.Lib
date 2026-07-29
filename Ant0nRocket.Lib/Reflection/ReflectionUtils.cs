using Ant0nRocket.Lib.Logging;
using Ant0nRocket.Lib.Patterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ant0nRocket.Lib.Reflection
{
    /// <summary>
    /// Collection of reflection utils with caching and thread-safety.<br />
    /// <b>Assembly loading after app domain created is not supported!!!</b>
    /// </summary>
    public static class ReflectionUtils
    {
        private static readonly ConcurrentDictionary<string, Type> _typeCache = [];

        /// <summary>
        /// Iterates over all loaded types in the AppDomain.
        /// If some assembly throws exception on .GetTypes() then the assembly will be skipped.
        /// </summary>
        private static void ForEachTypeInDomain(Action<Type> action)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                        action?.Invoke(type);
                }
                catch (Exception ex)
                {
                    // We will not stop app execution, but sent to logger anyway
                    Logger.LogException(ex);
                }
            }
        }

        private static void BuildTypeCache()
        {
            if (!_typeCache.IsEmpty) return; // we already built it
            ForEachTypeInDomain(t => {
                if (t.FullName != null) // we don't need generics, arrays, pointers, etc.
                    _ = _typeCache.TryAdd(t.FullName, t);
            });
        }

        static ReflectionUtils()
        {
            BuildTypeCache(); // will build type cache once and only once when first ReflectionUtils use
        }

        /// <summary>
        /// Finds a type by its full name in the current AppDomain.
        /// </summary>
        public static Result<Type> FindType(string typeFullName)
        {
            if (string.IsNullOrEmpty(typeFullName))
                return Result<Type>.Failure("Type name cannot be null or empty.");

            if (_typeCache.TryGetValue(typeFullName, out var type))
                return Result<Type>.Success(type);

            return Result<Type>.Failure($"Type '{typeFullName}' not found in app domain.");
        }

        /// <summary>
        /// Returns all types that implement or inherit from T.
        /// </summary>
        public static IEnumerable<Type> GetTypesThatImplements<T>()
        {
            var targetType = typeof(T);

            return _typeCache.Values
                .Where(t => t != targetType && targetType.IsAssignableFrom(t))
                .ToList(); // materialize to avoid double enumeration
        }

        /// <summary>
        /// Retrieves an attribute of type T from the given type.
        /// </summary>
        public static T? GetAttribute<T>(Type fromType) where T : Attribute
        {
            if (fromType == null)
                return default;

            try
            {
                return (T?)Attribute.GetCustomAttribute(fromType, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}