using Ant0nRocket.Lib.Std20.Attributes;
using Ant0nRocket.Lib.Std20.Logging;
using Ant0nRocket.Lib.Std20.Reflection;

using System;
using System.IO;
using System.Reflection;

namespace Ant0nRocket.Lib.Std20.IO
{
    public static class FileSystemUtils
    {
        private static readonly Logger _logger = Logger.Create(nameof(FileSystemUtils));

        

        /// <summary>
        /// The value is used to calculate result of <see cref="GetDefaultAppDataFolderPath"/>.<br />
        /// By default it is <see cref="Environment.SpecialFolder.LocalApplicationData"/> which
        /// leads to <i>%APPDATA%/Local</i>.
        /// </summary>
        public static Environment.SpecialFolder DefaultSpecialFolder { get; set; } =
            Environment.SpecialFolder.LocalApplicationData;

        /// <summary>
        /// Creates a directory <paramref name="path"/> if it doesn't exists.<br />
        /// Make sure you didn't provide a full file path here :)
        /// </summary>
        public static bool TouchDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    _logger.LogTrace($"Directory '{path}' wasn't exists. Created");
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, $"Unable to create directory '{path}': {ex.Message} ({ex.InnerException?.Message})");
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
            ReflectionUtils.AppName ??= Assembly.GetEntryAssembly().GetName().Name;
            var specialFolderPath = Environment.GetFolderPath(specialFolder);
            return Path.Combine(specialFolderPath, ReflectionUtils.AppName);
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
        /// it into T.<br />
        /// <b>N.B.!</b> If something goes wrong - a new instance of T will be returned
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
                    _logger.LogException(ex, $"Error while reading '{filePath}' into '{typeof(T).Name}'");
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
                _logger.LogException(ex);
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
                    _logger.LogException(ex);
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
                    _logger.LogException(ex);
                    return false;
                }
            }

            return false;
        }
    }
}
