using Ant0nRocket.Lib.Std20.DependencyInjection.Attributes;
using Ant0nRocket.Lib.Std20.IO;
using Ant0nRocket.Lib.Std20.IO.Serialization;
using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ant0nRocket.Lib.Std20.DependencyInjection
{
    /// <summary>
    /// Simple class for Dependency injection.<br />
    /// </summary>
    public static class DI
    {
        private static readonly Logger logger;

        private static readonly Dictionary<Type, object> singletones = new();

        private static readonly Dictionary<Type, byte[]> pushedSingltones = new();

        public static ISerializer BinarySerializer { get; private set; } = new BinarySerializer();
        public static ISerializer JsonSerializer { get; private set; } = new JsonSerializer();

        static DI()
        {
            logger = Logger.Create(nameof(DI));
        }

        #region Basic functionality

        public static T Get<T>()
        {
            var type = typeof(T);

            if (singletones.ContainsKey(type))
            {
                return (T)singletones[type];
            }
            else
            {
                // classes with TransientAttribute goes here
                return CreateAndRegisterInstance<T>();
            }
        }

        private static T CreateAndRegisterInstance<T>()
        {
            var type = typeof(T);

            var transientAttribute = GetAttribute<TransientAttribute>(type);
            if (transientAttribute != default)
            {
                // if transient - no singlotone!!!
                return CreateInstance<T>();
            }

            singletones.Add(type, default); // it doesn't exists, we checked on prev step
            var (canBeDeserialized, serializer, fullPath) = GetSerializerAndFilePath(type);
            if (canBeDeserialized)
            {
                try
                {
                    var (_, result) = serializer.ReadFromFileAndDeserialize<T>(fullPath);
                    CallInitializerMethodIfRequired(result);
                    singletones[type] = result;
                }
                catch (Exception e)
                {
                    logger.LogException(e, $"Can't deserialize type '{type}'");
                }
            }
            else
            {
                logger.LogTrace($"Deserialization is not about type '{type}'. New instance created");
                singletones[type] = CreateInstance<T>();
            }

            return (T)singletones[type];
        }

        private static T CreateInstance<T>()
        {
            var instance = Activator.CreateInstance<T>();
            CallInitializerMethodIfRequired(instance);
            return instance;
        }

        private static void CallInitializerMethodIfRequired<T>(T instance)
        {
            var type = typeof(T);
            var initializerMethodAtribute = GetAttribute<InitializerMethodAttribute>(type);
            // if nothing to dial with - return
            if (initializerMethodAtribute == default || initializerMethodAtribute.InitializerMethodName == default) return;

            var methods = type.GetMethods();
            if (!methods.Any(m => m.Name == initializerMethodAtribute.InitializerMethodName))
            {
                logger.LogWarning($"Type {type} has {nameof(InitializerMethodAttribute)} but proper method not found in instance");
                return;
            }

            methods.First(m => m.Name == initializerMethodAtribute.InitializerMethodName).Invoke(instance, null);
        }

        private static (bool Status, ISerializer Serializer, string FilePath) GetSerializerAndFilePath(Type type)
        {
            var saveAttribute = GetAttribute<SaveAttribute>(type);
            if (saveAttribute != default)
            {
                var serializer = saveAttribute.SerializerType == SerializerType.Json ?
                    JsonSerializer : BinarySerializer;

                var fileExtension = serializer.GetFileExtension();
                var fileName = Path.GetFileNameWithoutExtension(saveAttribute.FileName) + fileExtension;
                var fullPath = Path.Combine(
                    FileSystemUtils.GetAppDataPath(),
                    saveAttribute.DirectoryName,
                    fileName);
                return (true, serializer, fullPath);
            }
            return (false, default, default);
        }

        public static U GetAttribute<U>(Type type) where U : Attribute
        {
            var attribute = (U)Attribute.GetCustomAttribute(type, typeof(U));
            if (attribute == default)
                return default;            
            return attribute;
        }

        public static void Save<T>() => Save(typeof(T));

        private static void Save(Type type)
        {
            if (!singletones.ContainsKey(type))
            {
                logger.LogError($"Attempting to save unregistred type '{type}'. No action performed");
                return;
            }

            var (canBeSaved, serializer, filePath) = GetSerializerAndFilePath(type);
            if (!canBeSaved)
            {
                logger.LogTrace($"Instance of type '{type}' can't be saved");
                return;
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                logger.LogWarning($"No path is set for '{type}' to save it");
                return;
            }

            if (!serializer.SerializeAndSaveToFile(singletones[type], filePath))
            {
                logger.LogError($"Unable to save 'type'");
            }
        }

        public static void SaveAll()
        {
            foreach (var kvp in singletones)
            {
                var type = kvp.Key;
                Save(type);
            }
        }

        #endregion

        #region In-Memory backup

        /// <summary>
        /// Pushes binary formatted copy of instance into memory.
        /// </summary>
        public static void Push<T>()
        {
            var type = typeof(T);
            if (pushedSingltones.ContainsKey(type))
                throw new InvalidOperationException("only one push per-type allowed");

            if (!singletones.ContainsKey(type))
                throw new ArgumentException("T", $"{type} was not registred");

            using var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, singletones[type]);

            pushedSingltones.Add(type, memoryStream.ToArray());

            logger.LogDebug($"Instance of '{type}' has been pushed to memory");
        }

        /// <summary>
        /// Returns previously PUSHed copy of instance back to main cache and removes
        /// the copy from memory.
        /// </summary>
        public static void Pop<T>()
        {
            var type = typeof(T);
            if (!pushedSingltones.ContainsKey(type))
            {
                logger.LogWarning($"Unable to POP instance of '{type}': it wasn't pushed");
                return;
            }

            if (!singletones.ContainsKey(type))
                throw new ArgumentException("T", $"{type} was not registred");

            using var memoryStream = new MemoryStream(pushedSingltones[type]);
            var binaryFormatter = new BinaryFormatter();
            singletones[type] = binaryFormatter.Deserialize(memoryStream);
            pushedSingltones.Remove(type);

            logger.LogDebug($"Instance of '{type}' has been POPed from memory");
        }

        /// <summary>
        /// Removes a copy of instance from memory without restoring of original
        /// instance in main cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void PopKick<T>()
        {
            var type = typeof(T);
            if (!pushedSingltones.ContainsKey(type))
            {
                logger.LogWarning($"Unable to kick-off instance of '{type}': it wasn't PUSHed");
                return;
            }

            pushedSingltones.Remove(type);
            logger.LogDebug($"Instance of '{type}' was kicked-off from memory");
        }

        #endregion

        #region Danger, debug only functions

#if DEBUG
        /// <summary>
        /// Performs saving (if configured) of specified type and removes it from known singletones
        /// </summary>
        public static void Unload<T>()
        {
            var type = typeof(T);
            if (!singletones.ContainsKey(type))
            {
                logger.LogTrace($"No '{type}' registred. NOP");
                return;
            }

            Save<T>();
            singletones.Remove(type);
            logger.LogTrace($"Type '{type}' was removed from known types");
        }

        /// <summary>
        /// Performs reset of <typeparamref name="T"/> (new instance created)
        /// and returns it.<br />
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T Reset<T>()
        {
            var type = typeof(T);
            if (singletones.ContainsKey(type))
            {
                var targetType = singletones[type].GetType();
                singletones[type] = Activator.CreateInstance(targetType);
                return (T)singletones[type];
            }

            throw new ArgumentOutOfRangeException("T", $"{typeof(T)} is unknown");
        }
#endif

        #endregion
    }
}
