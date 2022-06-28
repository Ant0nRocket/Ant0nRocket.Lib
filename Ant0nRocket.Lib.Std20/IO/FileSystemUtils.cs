using Ant0nRocket.Lib.Std20.Attributes;
using Ant0nRocket.Lib.Std20.Logging;
using Ant0nRocket.Lib.Std20.Reflection;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Ant0nRocket.Lib.Std20.IO
{
    public static class FileSystemUtils
    {
        private static readonly Logger logger = Logger.Create(nameof(FileSystemUtils));

        #region Serializer

        private static ISerializer serializer = new DefaultSerializer();

        public static ISerializer GetSerializer() => serializer;

        public static void RegisterSerializer(ISerializer serializerInstance) =>
            serializer = serializerInstance;

        #endregion

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
        public static string GetDefaultAppDataFolderPathFor(string fileName, string subDirectory = default, Environment.SpecialFolder? specialFolder = null, bool autoTouchDirectory = false)
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
        public static T TryReadFromFile<T>(string filePath = default) where T : class
        {
            if (filePath == default)
            {
                var storeAttr = AttributeUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();
                filePath = GetDefaultAppDataFolderPathFor(storeAttr.FileName, storeAttr.DirectoryName);
            }

            T instance = default;

            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);
                instance = serializer.Deserialize<T>(fileContents);
            }

            if (instance == default)
                instance = Activator.CreateInstance<T>();

            return instance;
        }

        /// <summary>
        /// Tries save serialized <paramref name="instance"/> into <paramref name="filePath"/>.
        /// </summary>
        public static bool TrySaveToFile<T>(T instance, string filePath = default)
        {
            if (filePath == default)
            {
                var storeAttr = AttributeUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();

                if (string.IsNullOrEmpty(storeAttr.FileName)) return false;
                filePath = GetDefaultAppDataFolderPathFor(
                    storeAttr.FileName, storeAttr.DirectoryName, autoTouchDirectory: true);
            }

            var contents = serializer.Serialize(instance);

            try
            {
                var fileDirectoryPath = Path.GetDirectoryName(filePath);
                TouchDirectory(fileDirectoryPath);
                File.WriteAllText(filePath, contents);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
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
                    logger.LogException(ex);
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
                    logger.LogException(ex);
                    return false;
                }
            }

            return false;
        }
    }
}
