using System;
using System.Text.Encodings.Web;
using System.Text.Json;

using Ant0nRocket.Lib.Patterns;

namespace Ant0nRocket.Lib.Serialization
{
    /// <summary>
    /// Implementartion of standard Json serializer
    /// </summary>
    public class JsonStandardSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string contents, bool throwExceptions = false) where T : class, new()
        {
            return (T)Deserialize(contents, typeof(T), throwExceptions);
        }

        public object Deserialize(string contents, Type type, bool throwExceptions = false)
        {
            return System.Text.Json.JsonSerializer.Deserialize(contents, type) ?? new();
        }

        public string Serialize(object obj, bool pretty = false)
        {
            var options = new JsonSerializerOptions() { WriteIndented = pretty };

            return System.Text.Json.JsonSerializer.Serialize(obj, options);
        }
    }
}
