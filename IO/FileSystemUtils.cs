using Ant0nRocket.Lib.Std20.Logging;

using System;
using System.IO;
using System.Reflection;

namespace Ant0nRocket.Lib.Std20.IO
{
    public static class FileSystemUtils
    {
        private static readonly Logger logger;

        private static Assembly entryAssembly;
        private static Guid tempAppNameGuid = Guid.NewGuid();

        private static bool forcePortable = false;

        static FileSystemUtils()
        {
            logger = Logger.Create(nameof(FileSystemUtils));
        }

        public static void TouchDirectory(string path, bool pathContainsFileName = false)
        {            
            if (string.IsNullOrWhiteSpace(path)) return;

            if (pathContainsFileName)
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
                    logger.LogError($"Unable to create directory '{path}': {ex.Message} ({ex.InnerException?.Message})");
                }
            }
        }

        /// <summary>
        /// There are only one way for program to become portable: create ".portable" file in app folder
        /// </summary>
        public static bool IsPortable()
        {
            var appPath = AppDomain.CurrentDomain.BaseDirectory;
            var portableFilePath = Path.Combine(appPath, ".portable");
            return File.Exists(portableFilePath);
        }

        public static void SetPortableState(bool value) => forcePortable = value;

        public static string GetAppDataPath(AppDataFolder folder = AppDataFolder.UserLocalAppData, string subDirectory = default, bool autoCreateDirectory = true)
        {
            var targetDirectory = string.Empty;
            var isPortable = IsPortable() || forcePortable;

            if (isPortable)
            {
                targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                logger.LogTrace($"App in portable mode. AppDataPath='{targetDirectory}'");
            }
            else // normal, non-portable mode
            {
                switch (folder)
                {
                    case AppDataFolder.UserLocalAppData:
                        targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); break;

                    case AppDataFolder.UserRoamingAppData:
                        targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); break;

                    case AppDataFolder.Desktop:
                        targetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); break;

                    default:
                        throw new NotImplementedException();
                }
            }

            if (!isPortable)
            {
                var assembly = Assembly.GetEntryAssembly() ?? entryAssembly;
                if(assembly == default)
                {
                    var tempAppName = $"App-{tempAppNameGuid}";
                    logger.LogError($"Unable to get assembly. '{tempAppName}' will be used");
                    targetDirectory = Path.Combine(targetDirectory, tempAppName);
                }
                else
                {
                    var assemblyName = assembly.GetName();
                    targetDirectory = Path.Combine(targetDirectory, assemblyName.Name);
                }                
            }

#if DEBUG
            if (!targetDirectory.Contains("Debug"))
                targetDirectory = Path.Combine(targetDirectory, "Debug");
#endif

            if (subDirectory != default)
                targetDirectory = Path.Combine(targetDirectory, subDirectory);

            if (autoCreateDirectory)
                TouchDirectory(targetDirectory);

            return targetDirectory;
        }

        /// <summary>
        /// Required in Tests who can't provide Assembly.GetEntryAssembly().<br />
        /// Call it in test init like: FileSystemUtils.SetEntryAssembly(Assembly.GetCallingAssembly())
        /// </summary>
        /// <param name="assembly"></param>
        public static void SetEntryAssembly(Assembly assembly)
        {
            entryAssembly = assembly;
        }
    }
}
