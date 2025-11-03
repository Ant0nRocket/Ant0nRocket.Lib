using System;
using System.IO;

using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Configuration;
using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Helpers;
using Ant0nRocket.Lib.Patterns;
using Ant0nRocket.Lib.Reflection;

namespace Ant0nRocket.Lib.IO
{
    /// <summary>
    /// Collection of the file system utils
    /// </summary>
    public static class FileSystemUtils
    {
        /* There are two ways of storing app data:
         * (1) Portable mode:
           MyApp/
           ├── MyApp.exe
           ├── MyApp.dll
           ├── Data/                    <--- All data near the exe-file
           │   ├── Config/
           │   │   └── appsettings.json
           │   ├── Database/
           │   │   └── app.db
           │   ├── Logs/
           │   │   └── app_20231201.log
           │   ├── Cache/
           │   │   └── temp_files/
           │   └── Templates/
           │       └── default.tpl
           └── Plugins/
               └── custom_plugin.dll

           (2) Installed mode:
            # Windows
            C:\Users\{User}\AppData\Roaming\MyApp\
            ├── Config/
            ├── Database/
            ├── Logs/
            └── Cache/

            C:\Program Files\MyApp\          
            ├── MyApp.exe               <-- Executable file and libs (read-only!)
            └── Plugins/


        -------------------------------------------------------------------------------------------
        -------------------------------------------------------------------------------------------
        -------------------------------------------------------------------------------------------

        Inside this library, if the application has the right to write temp files in the current 
        base directory (AppContext.BaseDirectory - which is the place where executable file located) 
        then the application will operate in portable mode, otherwise - in installed mode.
         
         
         */

        /// <summary>
        /// Function determines whether app has an access to write to current app base directory
        /// (where exe-file located) or not.
        /// </summary>
        public static bool CanWriteToBaseDirectory() => CanWriteToDirectory(AppContext.BaseDirectory);

        /// <summary>
        /// Check write access to <paramref name="directoryPath"/>.
        /// </summary>
        public static bool CanWriteToDirectory(string directoryPath)
        {
            var tempFilePath = Path.Combine(directoryPath, Path.GetRandomFileName());

            try
            {
                File.WriteAllText(tempFilePath, tempFilePath);
                File.Delete(tempFilePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returnes the path to application data folder depending on <see cref="EnvironmentHelper.IsPortableMode"/>
        /// and respecting a flag <paramref name="isTransportableData"/> (if is heavy - then NO roaming!)
        /// </summary>
        public static string GetAppDataDirectory(bool isTransportableData = false)
        {
            if (EnvironmentHelper.IsPortableMode)
            {
                return Path.Combine(AppContext.BaseDirectory, "Data");
            }
            else
            {
                var specialFolder = isTransportableData ? 
                    Environment.SpecialFolder.ApplicationData : 
                    Environment.SpecialFolder.LocalApplicationData;
                var specialFolderPath = Environment.GetFolderPath(specialFolder);
                return Path.Combine(specialFolderPath, ApplicationInfo.ApplicationName);
            }
        }

        /// <summary>
        /// Shorthand for retreiving a config directory path.
        /// </summary>
        public static string GetAppDataDirectoryForConfig() => Path.Combine(GetAppDataDirectory(isTransportableData: true), "Config");

        /// <summary>
        /// Shorthand for retreiving a logs directory path.
        /// </summary>
        public static string GetAppDataDirectoryForLogs() => Path.Combine(GetAppDataDirectory(isTransportableData: false), "Logs");

        /// <summary>
        /// Creates a directory <paramref name="path"/>. 
        /// </summary>
        /// <returns>
        /// <see cref="Result{T}"/> where T - <see cref="DirectoryInfo"/>.
        /// </returns>
        public static Result<DirectoryInfo> TouchDirectory(string path)
        {
            try
            {
                // Create directory. It could exists. Anyway - return DirectoryInfo
                var directoryInfo = Directory.CreateDirectory(path);
                return Result<DirectoryInfo>.Success(directoryInfo);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error while directory creation: {ex.GetFullExceptionErrorMessage()}";
                return Result<DirectoryInfo>.Failure(errorMessage);
            }
        }





        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// The value is used to calculate result of <see cref="GetDefaultAppDataFolderPath"/>.<br />
        /// By default it is <see cref="Environment.SpecialFolder.LocalApplicationData"/> which
        /// leads to <i>%APPDATA%/Local</i>.
        /// </summary>
        public static Environment.SpecialFolder DefaultSpecialFolder { get; set; } =
            Environment.SpecialFolder.LocalApplicationData;








        /// <summary>
        /// Returnes app-dependent special folder.
        /// </summary>
        private static string GetAppNameDependentSpecialFolderPath(Environment.SpecialFolder specialFolder)
        {
            var specialFolderPath = Environment.GetFolderPath(specialFolder);
            return Path.Combine(specialFolderPath, ApplicationInfo.ApplicationName);
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
        /// You could change it by setting a new value to <see cref="DefaultSpecialFolder"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultAppDataFolderPath(Environment.SpecialFolder? specialFolder = null)
        {
            return Ant0nRocketLibConfig.IsPortableMode ?
                AppDomain.CurrentDomain.BaseDirectory :
                GetAppNameDependentSpecialFolderPath(specialFolder ?? DefaultSpecialFolder);
        }

        /// <summary>
        /// Will return valid data path for specified <paramref name="fileName"/>.<br />
        /// If <paramref name="subDirectory"/> specified - it will be added to data path.<br />
        /// If <paramref name="specialFolder"/> is default (<see cref="Environment.SpecialFolder.Fonts"/>) then
        /// <see cref="DefaultSpecialFolder"/> will be used.<br />
        /// If <paramref name="autoTouchDirectory"/> is true - data directory will be auto-created (if not exists).<br />
        /// <b>N.B.! If <see cref="IsPortableMode"/> then base app directory will be used. Don't forget to set <paramref name="subDirectory"/> in this case.</b>
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultAppDataFolderPathFor(string fileName, string? subDirectory = default, Environment.SpecialFolder? specialFolder = null, bool autoTouchDirectory = false)
        {
            var rootPath = GetDefaultAppDataFolderPath(specialFolder);

            subDirectory ??= string.Empty;

            var targetDirectory = Path.Combine(rootPath, subDirectory);

            if (autoTouchDirectory)
                TouchDirectory(targetDirectory);

            return Path.Combine(targetDirectory, fileName);
        }

        /// <summary>
        /// Tries read content of a <paramref name="filePath"/> and deserialize
        /// it into <typeparamref name="T"/>.
        /// </summary>
        public static T? TryReadFromFile<T>(string? filePath = default, bool createInstanceOnError = true) where T : class, new()
        {
            // This could throw so better call it here, before try block
            var jsonSerializer = Ant0nRocketLibConfig.GetJsonSerializer();

            if (filePath == default)
            {
                var storeAttr = ReflectionUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();
                filePath = GetDefaultAppDataFolderPathFor(storeAttr.FileName, storeAttr.DirectoryName);
            }

            T instance = default!;

            if (File.Exists(filePath))
            {
                try
                {
                    var fileContents = File.ReadAllText(filePath);
                    instance = jsonSerializer.Deserialize<T>(fileContents, throwExceptions: true);
                }
                catch (Exception ex)
                {
                    //_logger.LogException(ex, $"Error while reading '{filePath}' into '{typeof(T).Name}'");
                }
            }

            if (instance == default && createInstanceOnError)
            {
                instance = Activator.CreateInstance<T>();
            }

            return instance;
        }

        /// <summary>
        /// Tries save serialized <paramref name="instance"/> into <paramref name="filePath"/>.
        /// </summary>
        public static bool TrySaveToFile<T>(T instance, string? filePath = default, bool? backupOldData = default)
        {
            // This could throw so better call it here, before try block
            var jsonSerializer = Ant0nRocketLibConfig.GetJsonSerializer();

            var storeAttr = ReflectionUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();

            backupOldData ??= storeAttr.BackupOldData; // if not provided - take from attribute

            if (filePath == default)
            {
                if (string.IsNullOrEmpty(storeAttr.FileName) || string.IsNullOrWhiteSpace(storeAttr.FileName))
                {
                    throw new ApplicationException($"Neigther provide '{nameof(filePath)}' argument " +
                        $"or correct '{nameof(StoreAttribute)}' with {nameof(StoreAttribute.FileName)} specified");
                }

                filePath = GetDefaultAppDataFolderPathFor(
                    storeAttr.FileName, storeAttr.DirectoryName, autoTouchDirectory: true);
            }

            try
            {
                var contents = jsonSerializer.Serialize(instance!);
                var fileDirectoryPath = Path.GetDirectoryName(filePath);
                TouchDirectory(fileDirectoryPath);

                if (backupOldData == true && File.Exists(filePath))
                {
                    File.Copy(filePath, $"{filePath}.{DateTime.Now.Ticks}.bak");
                }

                File.WriteAllText(filePath, contents);
                return true;
            }
            catch (Exception ex)
            {
                //_logger.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Performs scanning of <paramref name="path"/>.<br />
        /// Every found filename goes to <paramref name="onFileFoundAction"/> so
        /// you could do whatever you want (make lists, do something with files, etc.)
        /// </summary>
        public static void ScanDirectoryRecursively(string path, Action<string> onFileFoundAction)
        {
            if (!Directory.Exists(path)) return;
            var files = Directory.GetFiles(path);
            foreach (var file in files) onFileFoundAction?.Invoke(file);

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
                ScanDirectoryRecursively(directory, onFileFoundAction);
        }


        /// <summary>
        /// Performs deleting of file or directory (in this case
        /// all files will be deleted recursively).
        /// Returnes true if deleted, othervise - false.
        /// </summary>
        public static bool Delete(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                    return true;
                }
                catch (Exception ex)
                {
                    //_logger.LogException(ex);
                    return false;
                }
            }

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch (Exception ex)
                {
                    //_logger.LogException(ex);
                    return false;
                }
            }

            return false;
        }
    }
}
