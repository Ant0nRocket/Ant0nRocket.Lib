using System;

namespace Ant0nRocket.Lib.Serialization
{
    /// <summary>
    /// Gateway between consumers of JSON serialization and library.
    /// </summary>
    public static class JsonSerialization
    {
        private static IJsonSerializer _jsonSerializer = new JsonStandardSerializer();

        /// <summary>
        /// Self-documented function name :)
        /// </summary>
        public static void RegisterJsonSerializer(IJsonSerializer? jsonSerializer) => 
            _jsonSerializer = jsonSerializer ?? throw new NullReferenceException();

        /// <summary>
        /// Self-documented function name :)
        /// </summary>
        /// <returns></returns>
        public static IJsonSerializer GetJsonSerializer() => _jsonSerializer;
    }
}
