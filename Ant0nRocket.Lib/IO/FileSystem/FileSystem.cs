using System;
using System.IO;
using System.Linq;

using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.IO.FileSystem.ReturnTypes;

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
        public static OneOf<Success, TouchDirectoryInvalidPath, TouchDirectoryUnauthorized, Error<Exception>> TouchDirectory(string? path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return new TouchDirectoryInvalidPath();

            for (var i = 0; i < path.Length; i++)
                if (Path.GetInvalidPathChars().Contains(path[i]))
                    return new TouchDirectoryInvalidPath();

            try
            {
                _ = Directory.CreateDirectory(path);
                return new Success();
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                    return new TouchDirectoryUnauthorized();

                var message = ex.GetFullExceptionErrorMessage();
                return new Error<Exception>(ex);
            }
        }
    }
}
