using System.Text.Encodings.Web;
using System.Text.Json;

namespace Ant0nRocket.Lib.Serialization
{
    /// <summary>
    /// Helper class that holds options for <see cref="System.Text.Json.JsonSerializer"/> options
    /// </summary>
    public static class JsonSerializerOptionsProvider
    {
        /// <summary>
        /// Read property name :)
        /// </summary>
        public static JsonSerializerOptions HumanReadable { get; } = 
            new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };

        /// <summary>
        /// Read property name :)
        /// </summary>
        public static JsonSerializerOptions Default => JsonSerializerOptions.Default;

        /// <summary>
        /// Will check 100% compatibility (even characters case).
        /// </summary>
        public static JsonSerializerOptions Strict => JsonSerializerOptions.Strict;

        /// <summary>
        /// Good for API (bring a lot of freedom in characters case, number interpretation, etc.)
        /// </summary>
        public static JsonSerializerOptions Web => JsonSerializerOptions.Web;
    }
}
