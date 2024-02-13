namespace Ant0nRocket.Lib.Std20.Logging
{
    /// <summary>
    /// Logging level of a current message
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Will appear on every level log
        /// </summary>
        All,

        /// <summary>
        /// Will appear on TRACE(1) level log
        /// </summary>
        Trace,

        /// <summary>
        /// Will appear on DEBUG(2) level log
        /// </summary>
        Debug,

        /// <summary>
        /// Will appear on INFO(3) level log
        /// </summary>
        Info,

        /// <summary>
        /// Will appear on WARN(4) level log
        /// </summary>
        Warn,

        /// <summary>
        /// Will appear on ERROR(5) level log
        /// </summary>
        Error,

        /// <summary>
        /// Will appear on FATAL(6) level log
        /// </summary>
        Fatal,

        /// <summary>
        /// Will NOT appear anywhere in log
        /// </summary>
        Off
    }
}
