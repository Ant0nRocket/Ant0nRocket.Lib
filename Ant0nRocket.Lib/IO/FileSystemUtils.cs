using System;
using System.IO;
using System.Text.Json;

using Ant0nRocket.Lib.Logging;
using Ant0nRocket.Lib.Reflection;

namespace Ant0nRocket.Lib.IO
{
    /// <summary>
    /// Collection of the file system utils.
    /// </summary>
    public static class FileSystemUtils
    {
        private const string SERIALIZED_FILES_EXT = ".json";

        /// <summary>
        /// Will generate a path with respect to <see cref="ReflectionUtils.CompanyName"/> and
        /// <see cref="ReflectionUtils.ApplicationName"/>, so that result will be:<br />
        /// [SpecialFolderPath] / [CompanyName] / [ApplicationName] / [SubFolder]<br /><br />
        /// Also, if <paramref name="autoCreate"/> set to true then folder will be created
        /// (if not exists).
        /// </summary>
        public static ReadOnlySpan<char> GetPath(Environment.SpecialFolder specialFolder, string? subfolder = default, bool autoCreate = true)
        {
            if (string.IsNullOrEmpty(ReflectionUtils.CompanyName))
                throw new ApplicationException(nameof(ReflectionUtils.CompanyName));

            if (string.IsNullOrEmpty(ReflectionUtils.ApplicationName))
                throw new ApplicationException(nameof(ReflectionUtils.ApplicationName));

            var specialFolderPath = Environment.GetFolderPath(specialFolder);
            var path = Path.Combine(
                specialFolderPath,
                ReflectionUtils.CompanyName,
                ReflectionUtils.ApplicationName,
                subfolder ?? string.Empty);

            if (autoCreate)
                TouchDirectory(path);

            return path.AsSpan();
        }

        /// <summary>
        /// Shorthand for getting logs folder.
        /// </summary>
        public static ReadOnlySpan<char> GetPathForData(string subfolder = "Data", bool autoCreate = true) =>
            GetPath(Environment.SpecialFolder.LocalApplicationData, subfolder, autoCreate);

        /// <summary>
        /// Shorthand for getting logs folder.
        /// </summary>
        public static ReadOnlySpan<char> GetPathForLogs(string subfolder = "Logs", bool autoCreate = true) =>
            GetPath(Environment.SpecialFolder.LocalApplicationData, subfolder, autoCreate);

        /// <summary>
        /// Shorthand for getting logs folder.
        /// </summary>
        public static ReadOnlySpan<char> GetPathForConfig(string subfolder = "Config", bool autoCreate = true) =>
            GetPath(Environment.SpecialFolder.ApplicationData, subfolder, autoCreate);


        /// <summary>
        /// Creates a directory <paramref name="path"/> if it doesn't exists.<br />
        /// Make sure you didn't provide a full file path here :)
        /// </summary>
        public static bool TouchDirectory(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (Directory.Exists(path)) return true;

            try
            {
                _ = Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Reads file <typeparamref name="T"/> from config folder (see <see cref=" GetPathForConfig(string, bool)"/>).
        /// </summary>
        public static T ReadConfigFile<T>() where T : class, new()
        {
            static T DeserializeFromFile(string path)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        var content = File.ReadAllText(path);
                        var instance = JsonSerializer.Deserialize<T>(content);
                        if (instance == default)
                        {
#if DEBUG
                            Logger.LogDebug($"Deserialization of {typeof(T)} from file '{path}' failed. New returned.");
#endif
                            return new();
                        }
                        else
                        {
                            return instance;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                    }
                }

                return new();
            }

            var type = typeof(T);
            var fileName = type.Name + SERIALIZED_FILES_EXT;
            var path = Path.Combine(GetPathForConfig().ToString(), fileName);
            if (File.Exists(path))
            {
                return DeserializeFromFile(path);
            }
            else
            {
#if DEBUG
                Logger.LogDebug($"File not found: {path}");
#endif
                return new();
            }

        }

        /// <summary>
        /// Save instance to config folder
        /// </summary>
        public static bool SaveConfigFile<T>(T instance)
        {
            var type = typeof(T);
            var fileName = type.Name + SERIALIZED_FILES_EXT;

            try
            {
                var savePath = Path.Combine(GetPathForConfig().ToString(), fileName);
                var serializedContent = JsonSerializer.Serialize(instance);
                File.WriteAllText(savePath, serializedContent);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Performs scanning of <paramref name="path"/>.<br />
        /// Every found filename goes to <paramref name="onFileFoundAction"/> so
        /// you could do whatever you want (make lists, do something with files, etc.)
        /// </summary>
        public static void ScanDirectoryRecursively(string path, Action<string>? onFileFoundAction)
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
