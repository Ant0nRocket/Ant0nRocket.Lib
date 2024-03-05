using System;
using System.Reflection;
using Ant0nRocket.Lib.IO.SignalBus;

namespace Ant0nRocket.Lib.Reflection
{
    /// <summary>
    /// Class contains "fire-and-forget" methods for reflections
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Current class will send messages to <see cref="SignalBus"/> with
        /// this channel specified.
        /// </summary>
        public const string SIGNAL_BUS_CHANNEL = nameof(Reflection);

        static Reflection()
        {
            _appName =
                Assembly.GetEntryAssembly()?.GetName()?.Name ??
                Guid.NewGuid().ToString();
            SignalBus.Send(_appName, SIGNAL_BUS_CHANNEL);
        }

        private static string? _appName = default;

        /// <summary>
        /// Application name that used across <see cref="Ant0nRocket.Lib"/>.
        /// </summary>
        public static string? AppName
        {
            get => _appName;
            set
            {
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                    _appName = value;
            }
        }
    }
}
