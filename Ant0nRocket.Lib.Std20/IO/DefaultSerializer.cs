using System;

namespace Ant0nRocket.Lib.Std20.IO
{
    internal class DefaultSerializer : ISerializer
    {
        public T Deserialize<T>(string contents)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);
        }

        public object Deserialize(string contents, Type type)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(contents, type);
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
