using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ant0nRocket.Lib.Std20.IO
{
    public static class FileSystemUtils
    {
        private static readonly Logger logger = Logger.Create(nameof(FileSystemUtils));

        private static string appName = default;

        /// <summary>
        /// Leave it default if you want an AppName from assembly name.
        /// </summary>
        public static string AppName
        {
            get
            {
                if (appName == default)
                    return Assembly.GetEntryAssembly().GetName().Name;
                return appName;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    logger.LogTrace($"AppName is '{AppName}'");
                    AppName = value;
                }
                else
                {
                    appName = default;
                }
            }
        }

        /// <summary>
        /// The value is used to calculate result of <see cref="GetDefaultAppDataFolderPath"/>.<br />
        /// By default it is <see cref="Environment.SpecialFolder.LocalApplicationData"/> which
        /// leads to <i>%APPDATA%/Local</i>.
        /// </summary>
        public static Environment.SpecialFolder DefaultAppDataFolder { get; set; } = 
            Environment.SpecialFolder.LocalApplicationData;

        /// <summary>
        /// Portable mode are usually auto-calculated but you can manually set it here.
        /// </summary>
        public static bool IsPortableMode { get; set; } = false;

        static FileSystemUtils()
        {
            AutoSetPortableState();
        }

        /// <summary>
        /// Automatically checks existance of the following conditions:<br />
        /// 1) Is ".portable" file exists near main executable?<br />
        /// 2) Is there a command line flag "--portable"?<br />
        /// 3) Are the application started from folder that contains "/Debug/" or ".Tests" in name?<br />
        /// If any of those conditions are true - portable mode will be set to true.
        /// </summary>
        private static void AutoSetPortableState()
        {
            var dsc = System.IO.Path.DirectorySeparatorChar; // "/" or "\"
            
            var appBaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            var portableFlagFilePath = Path.Combine(appBaseDirectoryPath, ".portable");
            
            var specialFilePortableFlag = File.Exists(portableFlagFilePath);
            var commandLinePortableFlag = Environment.GetCommandLineArgs().Contains("--portable");
            var appBaseDirectoryNamePortableFlag = 
                appBaseDirectoryPath.Contains($"{dsc}Debug{dsc}") || appBaseDirectoryPath.Contains($".Tests{dsc}");

            IsPortableMode = specialFilePortableFlag || commandLinePortableFlag || appBaseDirectoryNamePortableFlag;
        }

        /// <summary>
        /// Creates a directory <paramref name="path"/> if it doesn't exists.<br />
        /// Make sure you didn't provide a full file path here :)
        /// </summary>
        public static bool TouchDirectory(string path, bool endsWithFilename = false)
        {            
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (endsWithFilename)
                path = Path.GetDirectoryName(path);

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    logger.LogTrace($"Directory '{path}' wasn't exists. Created");
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, $"Unable to create directory '{path}': {ex.Message} ({ex.InnerException?.Message})");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returnes app-dependent special folder.
        /// </summary>
        private static string GetAppNameDependentSpecialFolderPath(Environment.SpecialFolder specialFolder)
        {
            AppName ??= Assembly.GetEntryAssembly().GetName().Name; 
            var specialFolderPath = Environment.GetFolderPath(specialFolder);
            return Path.Combine(specialFolderPath, AppName);
        }

        /// <summary>
        /// Returnes <i>%APPDATA%/Local/AppName</i> path.<br />
        /// <b>N.B.! Use it only if you realy want MANUALLY work with this folder, 
        /// othervide use <see cref="GetDefaultAppDataFolderPath"/></b>
        /// </summary>
        public static string GetAppDataLocalFolderPath() => GetAppNameDependentSpecialFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Returnes <i>%APPDATA%/Roaming/AppName</i> path.<br />
        /// <b>N.B.! Use it only if you realy want MANUALLY work with this folder, 
        /// othervide use <see cref="GetDefaultAppDataFolderPath"/></b>
        /// </summary>
        public static string GetAppDataRoamingFolderPath() => GetAppNameDependentSpecialFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Default app data folder path.<br />
        /// You could change it by setting a new value to <see cref="DefaultAppDataFolder"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultAppDataFolderPath() => 
            GetAppNameDependentSpecialFolderPath(DefaultAppDataFolder);

        /// <summary>
        /// If not <see cref="IsPortableMode>"/><br /><i>%APPDATA%/<see cref="DefaultAppDataFolder"/>/<see cref="AppName"/>/<paramref name="subDirectory"/>/<paramref name="fileName"/></i><br />othervise<br />
        /// <i><see cref="AppDomain.CurrentDomain.BaseDirectory"/>/<see cref="AppName"/></i> if <see cref="IsPortableMode"/>.
        /// </summary>
        public static string GetDefaultAppDataFolderPathFor(string fileName, string subDirectory = default) 
        {
            var rootPath = IsPortableMode ? AppDomain.CurrentDomain.BaseDirectory : GetDefaultAppDataFolderPath();
            subDirectory ??= string.Empty;

            return Path.Combine(rootPath, subDirectory, fileName);
        }
    }
}
