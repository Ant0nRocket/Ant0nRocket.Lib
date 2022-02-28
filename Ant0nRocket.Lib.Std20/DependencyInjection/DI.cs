using Ant0nRocket.Lib.Std20.DependencyInjection.Attributes;
using Ant0nRocket.Lib.Std20.IO;
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

        static DI()
        {
            logger = Logger.Create(nameof(DI));
        }

        #region Basic functionality

        public static void Register<T, U>() where U : class
        {
            var actualinterfaceType = typeof(T);
            if (!actualinterfaceType.IsInterface)
                throw new ArgumentException($"There should be an interface");

            CreateAndRememberInstance<U>(actualinterfaceType);
        }

        [Obsolete]
        public static T Get<T>()
        {
            var type = typeof(T);
            var transientAttribute = GetAttribute<TransientAttribute>(type);

            // Transient means that every Get() call should return new instance.
            // Nothing to store in singletones here.
            if (transientAttribute != null) return CreateInstance<T>();


            // From here - NON-TRANSIENT only!
            if (singletones.ContainsKey(type)) // if somewhere in the past we already create it...
            {
                return (T)singletones[type]; // then just return it.
            }
            else // type requested for the first time
            {
                return CreateAndRememberInstance<T>();
            }
        }

        private static T CreateAndRememberInstance<T>(Type type = default)
        {
            type ??= typeof(T);

            singletones.Add(type, CreateInstance<T>()); // something will be there, new instance or deserialized

            var saveAttribute = GetAttribute<SaveAttribute>(type);
            if (saveAttribute != default)
            {
                var filePath = FileSystemUtils.GetDefaultAppDataFolderPathFor(
                    fileName: saveAttribute.FileName,
                    subDirectory: saveAttribute.DirectoryName);

                if (File.Exists(filePath))
                {
                    try
                    {
                        var content = File.ReadAllText(filePath);
                        singletones[type] = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
                        if (singletones[type] == default)
                        {
                            singletones[type] = CreateInstance<T>();
                            logger.LogWarning($"File '{filePath}' has a bad contents. New instance of '{type}' created");
                        }
                        else
                        {
                            logger.LogDebug($"Instance of '{type}' deserialized from '{filePath}'");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(ex);
                    }
                }
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

            var saveAttribute = GetAttribute<SaveAttribute>(type);
            if (saveAttribute == default)
            {
                logger.LogWarning($"Type '{type}' doesn't have a SaveAttribute, saving canceled");
                return;
            }

            var filePath = FileSystemUtils.GetDefaultAppDataFolderPathFor(
                fileName: saveAttribute.FileName,
                subDirectory: saveAttribute.DirectoryName,
                autoTouchDirectory: true);

            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(singletones[type]);

            try
            {
                File.WriteAllText(filePath, contents);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, $"Unable to save content to file '{filePath}'");
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

        #region Danger functions

        /// <summary>
        /// Performs saving (if configured) of specified type and removes it from known singletones
        /// </summary>
        public static void Unload<T>(bool saveBeforeUnload = false)
        {
            var type = typeof(T);
            if (!singletones.ContainsKey(type))
            {
                logger.LogTrace($"No '{type}' registred. NOP");
                return;
            }

            if(saveBeforeUnload) Save<T>();
            singletones.Remove(type);
            logger.LogTrace($"Type '{type}' was removed from singletones collection");
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

        #endregion
    }
}
