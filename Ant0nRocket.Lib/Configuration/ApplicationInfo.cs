using System;
using System.Reflection;

namespace Ant0nRocket.Lib.Configuration
{
    /// <summary>
    /// Basic information about the application: name, version, etc.
    /// </summary>
    public static class ApplicationInfo
    {
        private static string? _applicationName = default;

        /// <summary>
        /// Gets or sets current application name. This information is required
        /// for application data folder path calculation and other application-specific paths.
        /// If not set explicitly, automatically resolves from assembly metadata.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when value is null or whitespace</exception>
        public static string ApplicationName
        {
            get
            {
                if (_applicationName == default)
                    _applicationName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? $"App_{Guid.NewGuid():N}";
                return _applicationName;
            }
            set => _applicationName = string.IsNullOrWhiteSpace(value)
                ? throw new ArgumentException("Application name cannot be null or empty", nameof(value))
                : value;
        }

        /// <summary>
        /// Gets whether application is running in debug mode.
        /// </summary>
        public static bool IsDebugMode
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets whether application is running in release mode.
        /// </summary>
        public static bool IsReleaseMode => !IsDebugMode;

        /// <summary>
        /// Gets the application version from entry assembly.
        /// </summary>
        public static Version? Version => Assembly.GetEntryAssembly()?.GetName().Version;

        /// <summary>
        /// Gets the application version as string (major.minor.build format).
        /// </summary>
        public static string VersionString => Version?.ToString(3) ?? "1.0.0";
    }
}
