using System;

namespace Ant0nRocket.Lib.Std20.IO
{
    public interface ISerializer
    {
        T Deserialize<T>(string contents);

        object Deserialize(string contents, Type type);

        string Serialize(object obj);
    }
}
