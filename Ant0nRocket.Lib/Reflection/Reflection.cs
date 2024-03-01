using System;
using System.Reflection;

namespace Ant0nRocket.Lib.Reflection
{
    /// <summary>
    /// Class contains "fire-and-forget" methods for reflections
    /// </summary>
    public static class Reflection
    {
        private static string? _appName = default;

        /// <summary>
        /// Allows you to set app name manually.
        /// If you never set the app name - reflections will get it from Assembly
        /// </summary>
        public static bool SetAppName(string? appName)
        {
            if (string.IsNullOrEmpty(appName) || string.IsNullOrWhiteSpace(appName))
                return false;                
            _appName = appName;
            return true;
        }

        /// <summary>
        /// If AppName were set by <see cref="SetAppName(string?)"/> then
        /// the value will be returned.
        /// If AppName is null then <see cref="Assembly.GetEntryAssembly"/> name
        /// will be returned.
        /// If nothing to return - new <see cref="Guid"/> (as string) returned.
        /// </summary>
        public static string GetAppName()
        {
            if (_appName != default)
                return _appName;

            return 
                Assembly.GetEntryAssembly()?.GetName()?.Name ?? 
                Guid.NewGuid().ToString();
        }
    }
}
