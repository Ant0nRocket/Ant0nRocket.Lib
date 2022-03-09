namespace Ant0nRocket.Lib.Std20.IO
{
    internal class DefaultSerializer : ISerializer
    {
        public T Deserialize<T>(string contents)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(contents);
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
