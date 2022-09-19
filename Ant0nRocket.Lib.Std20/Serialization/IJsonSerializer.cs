using System;

namespace Ant0nRocket.Lib.Std20.Serialization
{
    /// <summary>
    /// Basic JSON serializer used in Ant0nRocket.Lib.Std20
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Try deserialize <paramref name="contents"/> into <typeparamref name="T"/>.
        /// If <paramref name="throwExceptions"/> then on error exception will be thrown,
        /// else - new instance of <typeparamref name="T"/> will be created.
        /// </summary>
        T Deserialize<T>(string contents, bool throwExceptions = false) where T : class, new();

        /// <summary>
        /// Same as <see cref="Deserialize{T}(string, bool)"/> but type specified manually.
        /// Usefull when runtime deserialization take place. Same exceptions policy.
        /// </summary>
        object Deserialize(string contents, Type type, bool throwExceptions = false);

        /// <summary>
        /// Serializes <paramref name="obj"/> into string JSON. If <paramref name="pretty"/> is true
        /// then intending will be applied to text.
        /// </summary>
        string Serialize(object obj, bool pretty = false);
    }
}
