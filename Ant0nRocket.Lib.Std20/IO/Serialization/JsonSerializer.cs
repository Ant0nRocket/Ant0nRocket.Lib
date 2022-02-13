using Ant0nRocket.Lib.Std20.Logging;

using Newtonsoft.Json;

using System;
using System.IO;

namespace Ant0nRocket.Lib.Std20.IO.Serialization
{
    public class JsonSerializer : SerializerBase, ISerializer
    {
        public JsonSerializer() : base(Logger.Create<JsonSerializer>()) { }

        public string GetFileExtension() => ".json";

        public bool IsBinary => false;

        public override T Deserialize<T>(string path)
        {
            var contents = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(contents);
        }

        public object Deserialize(string contents, Type type)
        {
            return JsonConvert.DeserializeObject(contents, type);
        }

        public override void Serialize(object obj, string path)
        {
            var serializedObject = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, serializedObject);
        }

        
    }
}
