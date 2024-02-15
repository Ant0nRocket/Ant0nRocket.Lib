using Ant0nRocket.Lib.Serialization;
using System;
using System.Text.Json;

namespace Ant0nRocket.Lib.Tests.Serialization
{
    public class JsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string contents, bool throwExceptions = false) where T : class, new()
        {
            try
            {
                var instance = System.Text.Json.JsonSerializer.Deserialize<T>(contents);
                if (instance == null)
                {
                    if (throwExceptions)
                    {
                        throw new InvalidOperationException("Can't deserialize string");
                    }
                }
                else
                {
                    return instance!;
                }
            }
            catch (Exception ex)
            {
                if (throwExceptions)
                    throw new Exception("See inner exception", ex);
            }

            return Activator.CreateInstance<T>();
        }

        public object Deserialize(string contents, Type type, bool throwExceptions)
        {
            try
            {
                var instance = System.Text.Json.JsonSerializer.Deserialize(contents, type);
                if (instance == null && throwExceptions)
                    throw new InvalidOperationException("Can't deserialize string");
            }
            catch (Exception ex)
            {
                if (throwExceptions)
                    throw new Exception("See inner exception", ex);
            }

            return Activator.CreateInstance(type)!;
        }

        public string Serialize(object obj, bool pretty = false)
        {
            var options = new JsonSerializerOptions { WriteIndented = pretty };
            return System.Text.Json.JsonSerializer.Serialize(obj, obj.GetType(), options);
        }
    }
}
