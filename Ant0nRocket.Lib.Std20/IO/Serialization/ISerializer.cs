using System;

namespace Ant0nRocket.Lib.Std20.IO.Serialization
{
    public interface ISerializer
    {
        bool SerializeAndSaveToFile(object obj, string path);

        (bool Read, T Result) ReadFromFileAndDeserialize<T>(string path);

        T Deserialize<T>(string path);

        object Deserialize(string contents, Type type);

        void Serialize(object obj, string path);

        string GetFileExtension();     
        
        bool IsBinary { get; }
    }
}
