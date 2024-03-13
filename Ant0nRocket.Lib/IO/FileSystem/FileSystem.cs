using System;
using System.IO;
using System.Linq;

using Ant0nRocket.Lib.IO.FileSystem.ReturnTypes;
using Reflection = Ant0nRocket.Lib.Reflection;
using OneOf;
using OneOf.Types;

namespace Ant0nRocket.Lib.IO.FileSystem
{
    /// <summary>
    /// Collection of "fire-and-forget" file system utils
    /// </summary>
    public static class FileSystem
    {
        /// <summary>
        /// Creates a directory specified in <paramref name="path"/>.
        /// If there were no errors (or directory exists) - <see cref="Success"/> returned.
        /// Othervise - <see cref="Error{T}"/>, with error message inside. 
        /// </summary>
        public static OneOf<Success<string>, TouchDirectoryInvalidPath, TouchDirectoryUnauthorized, Error<Exception>> TouchDirectory(string? path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return new TouchDirectoryInvalidPath();

            for (var i = 0; i < path.Length; i++)
                if (Path.GetInvalidPathChars().Contains(path[i]))
                    return new TouchDirectoryInvalidPath();

            try
            {
                var directoryInfo = Directory.CreateDirectory(path);
                return new Success<string>(directoryInfo.FullName);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                    return new TouchDirectoryUnauthorized();
                return new Error<Exception>(ex);
            }
        }

        /// <summary>
        /// If app is in <see cref="Reflection.IsPortableMode"/> then
        /// current domain base directory will be returned.<br />
        /// Othervise (if not portable) - '~/User/.AppName' returned.<br />
        /// Pay attension, it is a period symbol before AppName.
        /// </summary>
        public static string GetAppDataPath()
        {
            var isPortableMode = Reflection.IsPortableMode;

            return "";
        }

    }
}
