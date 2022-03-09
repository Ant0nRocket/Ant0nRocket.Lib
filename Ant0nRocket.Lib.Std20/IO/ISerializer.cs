namespace Ant0nRocket.Lib.Std20.IO
{
    public interface ISerializer
    {
        T Deserialize<T>(string contents);

        string Serialize(object obj);
    }
}
