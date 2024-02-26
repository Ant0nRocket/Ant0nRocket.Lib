using System;

namespace Ant0nRocket.Lib.Logging
{
    /// <summary>
    /// Single log event entity
    /// </summary>
    public record LogEntity
    {
        /// <summary>
        /// Contains a message to log (string.Empty by default)
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Level on which current message will appear
        /// </summary>
        public LogLevel LogLevel { get; init; } = LogLevel.All;

        /// <summary>
        /// Date and time (UTC) when this message were created
        /// </summary>
        public DateTime DateTimeUtc { get; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time (local) when this message were created
        /// </summary>
        public DateTime DateTimeLocal => DateTimeUtc.ToLocalTime();

        /// <summary>
        /// Allowes user to set some integer tag information to, say,
        /// filter log messages in <see cref="ILogEntityHandler"/>.
        /// User must control tag uniquity itself.
        /// </summary>
        public int Tag { get; set; }

        /// <summary>
        /// Id of a thread in which the entity created.
        /// </summary>
        public int ThreadId { get; init; }

#if DEBUG

        /// <summary>
        /// Name of the sender class
        /// </summary>
        public string? SenderClassName { get; init; } = default;

        /// <summary>
        /// Name of of method that send current entity
        /// </summary>
        public string? SenderMethodName { get; init; } = default;

#endif
    }
}
