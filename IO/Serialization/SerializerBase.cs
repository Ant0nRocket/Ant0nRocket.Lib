using Ant0nRocket.Lib.Std20.Diagnostic;
using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.IO;

namespace Ant0nRocket.Lib.Std20.IO.Serialization
{
    public abstract class SerializerBase
    {
        protected readonly Logger logger;

        public SerializerBase(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Each serializer should now what to do with file.<br />
        /// Function already try-catch protected!
        /// </summary>
        public abstract T Deserialize<T>(string path);

        public abstract void Serialize(object obj, string path);

        public (bool Read, T Result) ReadFromFileAndDeserialize<T>(string path)
        {
            if (!File.Exists(path))
            {
                logger.LogWarning($"File '{path}' doesn't exists. New instance created");
                return (false, Activator.CreateInstance<T>());
            }

            try
            {
                T obj = default;
                new ExecTimeTracker().Track($"Deserialization of file '{path}'", () => {
                    obj = Deserialize<T>(path);
                });                
                logger.LogInformation($"Instance of '{typeof(T)}' successfuly read from '{path}'");
                return (true, obj);
            }
            catch (Exception e)
            {
                logger.LogException(e, $"Unable to deserialize '{typeof(T)}' from file '{path}'. New instance created");
                return (false, Activator.CreateInstance<T>());
            }
        }

        public bool SerializeAndSaveToFile(object obj, string path)
        {
            if (obj == default) return false;

            var directory = Path.GetDirectoryName(path);
            FileSystemUtils.TouchDirectory(directory);

            try
            {
                new ExecTimeTracker().Track($"Serialization of '{obj.GetType()}'", () => {
                    Serialize(obj, path);
                });
                logger.LogTrace($"Instance of '{obj.GetType()}' saved to '{path}'");
                return true;
            }
            catch (Exception e)
            {
                logger.LogException(e, $"Unable to save serialized '{obj.GetType()}' to '{path}'");
                return false;
            }
        }
    }
}
