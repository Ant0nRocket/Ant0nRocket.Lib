using System;
using System.IO;
using System.Linq;

namespace Ant0nRocket.Lib.Helpers
{
    /// <summary>
    /// Extended environment utilities that complement <see cref="Environment"/> class functionality.
    /// </summary>
    public static class EnvironmentHelper
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
        /// <inheritdoc cref="IsPortableMode"/>
        /// </summary>
        public static bool IsInstalledMode => !IsPortableMode;

        /// <summary>
        /// Automatically checks existence of the following conditions:<br />
        /// 1) Does ".portable" file exist near main executable?<br />
        /// 2) Is there a command line flag "--portable"?<br />
        /// 3) Was the application started from folder that contains "/Debug/" in name?<br />
        /// 4) If there is an ability to write file in current BaseDirectory<br />
        /// If any of those conditions are true - portable mode is set to true.
        /// </summary>
        public static bool IsPortableMode =>
            ForcePortableMode == default ? _isPortable.Value : ForcePortableMode!.Value; // ForcePortableMode can't be null here

        public static bool? ForcePortableMode { get; set; } = default;

        private static readonly Lazy<bool> _isPortable = new(() =>
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Any(arg => string.Equals(arg, "--portable", StringComparison.OrdinalIgnoreCase)))
                return true;

            var appBaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

            if (appBaseDirectoryPath.Contains("/Debug/", StringComparison.OrdinalIgnoreCase) ||
                appBaseDirectoryPath.Contains("\\Debug\\", StringComparison.OrdinalIgnoreCase))
                return true;

            var portableFilePath = Path.Combine(appBaseDirectoryPath, ".portable");
            if (File.Exists(portableFilePath))
                return true;

            var testFilePath = Path.Combine(appBaseDirectoryPath, $"{Guid.NewGuid()}.tmp");
            try
            {
                File.WriteAllText(testFilePath, testFilePath);
                File.Delete(testFilePath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        });

        /// <summary>
        /// Same as <see cref="AppContext.BaseDirectory"/>
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory() => AppContext.BaseDirectory;

        #endregion
    }
}
