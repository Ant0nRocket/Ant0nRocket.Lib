using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Configuration;
using Ant0nRocket.Lib.Exceptions;
using Ant0nRocket.Lib.Helpers;
using Ant0nRocket.Lib.Logging;
using Ant0nRocket.Lib.Patterns;
using Ant0nRocket.Lib.Reflection;
using Ant0nRocket.Lib.Serialization;
using System;
using System.IO;
using System.Text.Json;

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
        /// Performs backing up of specified file into same directory or <paramref name="subfolder"/>
        /// if provided.
        /// </summary>
        private static Result BackupFile(string sourceFilePath, string? subfolder = "Backup")
        {
            if (!File.Exists(sourceFilePath))
                return Result.Failure($"Source file '{sourceFilePath}' not found");

            var (sourceFileDirectoryName, sourceFileNameWithoutExt, sourceFileExt) = GetFilePathParts(sourceFilePath);
            var backupFileDirectoryName = Path.Combine(sourceFileDirectoryName, subfolder ?? string.Empty);

            if (TouchDirectory(backupFileDirectoryName).IsFailure)
                return Result.Failure($"Can't touch directory '{backupFileDirectoryName}'");

            var backupFilePath = Path.Combine(
                backupFileDirectoryName,
                $"{sourceFileNameWithoutExt}_{DateTime.Now.Ticks}{sourceFileExt}");

            try
            {
                File.Copy(sourceFilePath, backupFilePath, overwrite: false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex);
            }
        }

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
        /// Returnes the path to application data folder depending on <see cref="EnvironmentHelper.IsPortableMode"/>.
        /// If app is portable then everything will be stored inside "Data" directory (we respect user PC and don't put any
        /// trash anywhere in the system!).
        /// Same for normal mode: heavy data - in local %app_data% (to prevent roaming), else - in roaming %app_data%
        /// </summary>
        public static string GetDataDirectoryName(bool isTransportableData = false)
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
        /// Returnes path packed in single cortage
        /// </summary>
        public static (string directoryName, string fileNameWithoutExt, string extension) GetFilePathParts(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);

            return (
                directoryName ?? string.Empty,
                fileNameWithoutExt ?? string.Empty,
                extension ?? string.Empty);
        }

        /// <summary>
        /// Creates a directory <paramref name="path"/>. 
        /// </summary>
        /// <returns>
        /// <see cref="Result{T}"/> where T - <see cref="DirectoryInfo"/>.
        /// </returns>
        public static Result<DirectoryInfo> TouchDirectory(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return Result<DirectoryInfo>.Failure("Null path provided");

            try
            {
                var directoryInfo = Directory.CreateDirectory(path);
                return Result<DirectoryInfo>.Success(directoryInfo);
            }
            catch (Exception ex)
            {
                return Result<DirectoryInfo>.Failure(ex);
            }
        }

        /// <summary>
        /// Reads a JSON file and attempts to deserialize it into <typeparamref name="T"/>.
        /// </summary>
        /// <param name="filePath">Path to the JSON file.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> containing the deserialized instance on success,
        /// or an error if the file could not be read or deserialized.
        /// </returns>
        public static Result<T> LoadFromJsonFile<T>(string filePath) where T : class
        {
            try
            {
                using var fileStream = File.OpenRead(filePath);
                var instance = JsonSerializer.Deserialize<T>(fileStream);
                if (instance != null)
                    return Result<T>.Success(instance);

                return Result<T>.Failure($"Unable to deserialize content of '{filePath}' to {typeof(T).Name}");
            }
            catch (Exception ex) // checked - Exception is ok here
            {
                return Result<T>.Failure(ex);
            }
        }

        /// <summary>
        /// Wrapper around <see cref="LoadFromJsonFile{T}(string)"/> with two differences:<br />
        /// 1) If deserialization failed then new instance will be returned;<br />
        /// 2) Only [Store] decorated classes could be used (<see cref="AppDataLocationAttribute" />)
        /// </summary>
        public static T LoadOrCreateFromAppData<T>() where T : class, new()
        {
            var storeAttr = ReflectionUtils.GetAttribute<AppDataLocationAttribute>(typeof(T)) ??
                throw new MissingAppDataLocationAttributeException($"Type '{typeof(T).Name}' must be decorated with {nameof(AppDataLocationAttribute)}");

            var filePath = Path.Combine(GetDataDirectoryName(), storeAttr.SubdirectoryName, storeAttr.FileName);

            var result = LoadFromJsonFile<T>(filePath);
            if (result.IsSuccess)
                return result.Value;

            Logger.LogError(result.Error!);
            return new();
        }

        /// <summary>
        /// Writes serialized <paramref name="instance"/> to file which path depends on <see cref="AppDataLocationAttribute"/>.
        /// </summary>
        public static Result SaveToAppData<T>(T instance,
            string? destFilePath = null,
            bool backupOldData = true,
            string backupOldDataSubfolder = "Backup",
            JsonSerializerOptions? jsonSerializerOptions = default)
        {
            var storeAttr = ReflectionUtils.GetAttribute<AppDataLocationAttribute>(typeof(T)) ??
                throw new ApplicationException($"Type '{typeof(T)}' must be decorated with {nameof(AppDataLocationAttribute)}");

            var dataDirectory = GetDataDirectoryName();

            destFilePath ??= Path.Combine(dataDirectory, storeAttr.SubdirectoryName, storeAttr.FileName);
            var destFileDirectory = Path.GetDirectoryName(destFilePath);

            var touchDestFileDirectoryResult = TouchDirectory(destFileDirectory);
            if (touchDestFileDirectoryResult.IsSuccess)
            {
                if (File.Exists(destFilePath) && backupOldData)
                {
                    var backupDestFileResult = BackupFile(destFilePath, backupOldDataSubfolder);
                    if (backupDestFileResult.IsFailure)
                        return backupDestFileResult;
                }

                try // Main part
                {
                    var contents = JsonSerializer.Serialize(instance,
                        jsonSerializerOptions ?? JsonSerializerOptionsProvider.HumanReadable);

                    File.WriteAllText(destFilePath, contents);
                    return Result.Success();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    return Result.Failure(ex);
                }
            }
            else
            {
                return Result.Failure(touchDestFileDirectoryResult.Error);
            }
        }

        /// <summary>
        /// Performs scanning of <paramref name="path"/>.<br />
        /// Every found filename goes to <paramref name="onFileFoundAction"/> so
        /// you could do whatever you want (make lists, do something with files, etc.)
        /// </summary>
        public static void ScanDirectoryRecursively(string path, Action<string>? onFileFoundAction)
        {
            if (onFileFoundAction == default) return; // no action with found paths - no scan

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
                    Logger.LogException(ex);
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
                    Logger.LogException(ex);
                    return false;
                }
            }

            return false;
        }
    }
}
