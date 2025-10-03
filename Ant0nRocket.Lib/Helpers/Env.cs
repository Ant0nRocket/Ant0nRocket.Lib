using System;
using System.IO;
using System.Linq;

namespace Ant0nRocket.Lib.Helpers
{
    /// <summary>
    /// Class that extands functionality of <see cref="Environment"/>.
    /// </summary>
    public static class Env
    {
        #region Standard Environment properties

        /// <inheritdoc cref="Environment.CommandLine"/>
        public static string CommandLine => Environment.CommandLine;

        /// <inheritdoc cref="Environment.CurrentDirectory"/>
        public static string CurrentDirectory => Environment.CurrentDirectory;

        /// <inheritdoc cref="Environment.Is64BitProcess"/>
        public static bool Is64BitProcess => Environment.Is64BitProcess;

        /// <inheritdoc cref="Environment.MachineName"/>
        public static string MachineName => Environment.MachineName;

        /// <inheritdoc cref="Environment.OSVersion"/>
        public static OperatingSystem OSVersion => Environment.OSVersion;

        /// <inheritdoc cref="Environment.ProcessorCount"/>
        public static int ProcessorCount => Environment.ProcessorCount;

        /// <inheritdoc cref="Environment.UserName"/>
        public static string UserName => Environment.UserName;

        /// <inheritdoc cref="Environment.Version"/>
        public static Version Version => Environment.Version;

        /// <inheritdoc cref="Environment.WorkingSet"/>
        public static long WorkingSet => Environment.WorkingSet;

        #endregion

        #region Extended properties and functions

        /// <summary>
        /// Automatically checks existance of the following conditions:<br />
        /// 1) Is ".portable" file exists near main executable?<br />
        /// 2) Is there a command line flag "--portable"?<br />
        /// 3) Are the application started from folder that contains "/Debug/" in name?<br />
        /// If any of those conditions are true - portable mode will be set to true.
        /// </summary>
        public static bool IsPortable => _isPortable.Value;

        private static readonly Lazy<bool> _isPortable = new(() =>
        {
            // Сначала проверяем самые быстрые условия
            var args = Environment.GetCommandLineArgs();
            if (args.Any(arg => string.Equals(arg, "--portable", StringComparison.OrdinalIgnoreCase)))
                return true;

            var appBaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

            if (appBaseDirectoryPath.Contains("/Debug/", StringComparison.OrdinalIgnoreCase) ||
                appBaseDirectoryPath.Contains("\\Debug\\", StringComparison.OrdinalIgnoreCase))
                return true;

            // File.Exists - самая медленная операция, проверяем последней
            var portableFilePath = Path.Combine(appBaseDirectoryPath, ".portable");
            return File.Exists(portableFilePath);
        });

        #endregion
    }
}
