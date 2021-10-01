using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Ant0nRocket.Lib.Std20.IO.Serialization
{
    public class BinarySerializer : SerializerBase, ISerializer
    {
        public BinarySerializer() : base(Logger.Create<BinarySerializer>()) { }

        public string GetFileExtension() => ".dat";

        public bool IsBinary => true;

        public override T Deserialize<T>(string path)
        {
            using var readStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            var binaryFormatter = new BinaryFormatter();
            return (T)binaryFormatter.Deserialize(readStream);
        }

        public override void Serialize(object obj, string path)
        {
            using var writeStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(writeStream, obj);
        }

        public object Deserialize(string contents, Type type)
        {
            throw new NotImplementedException();
        }
    }
}
