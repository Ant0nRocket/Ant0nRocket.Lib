﻿using Ant0nRocket.Lib.Std20.Attributes;
using Ant0nRocket.Lib.Std20.Logging;
using Ant0nRocket.Lib.Std20.Reflection;

using System;
using System.IO;
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
        public static Environment.SpecialFolder DefaultSpecialFolder { get; set; } =
            Environment.SpecialFolder.LocalApplicationData;

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
        /// You could change it by setting a new value to <see cref="DefaultSpecialFolder"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultAppDataFolderPath() =>
            GetAppNameDependentSpecialFolderPath(DefaultSpecialFolder);

        /// <summary>
        /// Will return valid data path for specified <paramref name="fileName"/>.<br />
        /// If <paramref name="subDirectory"/> specified - it will be added to data path.<br />
        /// If <paramref name="specialFolder"/> is default (<see cref="Environment.SpecialFolder.Fonts"/>) then
        /// <see cref="DefaultSpecialFolder"/> will be used.<br />
        /// If <paramref name="autoTouchDirectory"/> is true - data directory will be auto-created (if not exists).<br />
        /// <b>N.B.! If <see cref="IsPortableMode"/> then base app directory will be used. Don't forget to set <paramref name="subDirectory"/> in this case.</b>
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultAppDataFolderPathFor(string fileName, string subDirectory = default, Environment.SpecialFolder specialFolder = Environment.SpecialFolder.Fonts, bool autoTouchDirectory = false)
        {
            specialFolder = specialFolder == Environment.SpecialFolder.Fonts ?
                specialFolder = DefaultSpecialFolder : specialFolder;

            var rootPath = Ant0nRocketLibConfig.IsPortableMode ?
                AppDomain.CurrentDomain.BaseDirectory : GetAppNameDependentSpecialFolderPath(specialFolder);

            subDirectory ??= string.Empty;

            var targetDirectory = Path.Combine(rootPath, subDirectory);

            if (autoTouchDirectory)
                TouchDirectory(targetDirectory);

            return Path.Combine(targetDirectory, fileName);
        }

        public static T TryReadFromFile<T>() where T : class
        {
            var storeAttr = AttributeUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();
            var filePath = GetDefaultAppDataFolderPathFor(storeAttr.FileName, storeAttr.DirectoryName);
            T instance = default;

            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);
                instance = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(fileContents);
            }

            if (instance == default)
                instance = (T)Activator.CreateInstance<T>();

            return instance;
        }

        public static bool TrySaveToFile<T>(T instance)
        {
            var storeAttr = AttributeUtils.GetAttribute<StoreAttribute>(typeof(T)) ?? new();

            if (string.IsNullOrEmpty(storeAttr.FileName)) return false;

            var filePath = GetDefaultAppDataFolderPathFor(
                storeAttr.FileName, storeAttr.DirectoryName, autoTouchDirectory: true);

            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(instance);

            try
            {
                File.WriteAllText(filePath, contents);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }
        }
    }
}
