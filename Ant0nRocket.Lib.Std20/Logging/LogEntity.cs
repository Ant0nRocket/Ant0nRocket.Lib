using System;

namespace Ant0nRocket.Lib.Std20.Logging
{
    /// <summary>
    /// Single log event entity
    /// </summary>
    public record LogEntity
    {
        /// <summary>
        /// Contains a message to log (string.Empty by default)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Level on which current message will appear
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.All;

        /// <summary>
        /// Date and time (UTC) when this message were created
        /// </summary>
        public DateTime DateTimeUtc { get; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time (local) when this message were created
        /// </summary>
        public DateTime DateTimeLocal => DateTimeUtc.ToLocalTime();
    }
}
