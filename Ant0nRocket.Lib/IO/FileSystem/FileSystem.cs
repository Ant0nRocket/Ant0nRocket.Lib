using Ant0nRocket.Lib.Extensions;
using OneOf;
using OneOf.Types;
using System;
using System.IO;
using System.Linq;

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
        public static OneOf<Success, Error<string>> TouchDirectory(string? path)
        {
            const string ERR_EMPTY_PATH = "Provided path is empty";
            const string ERR_INVALID_SYMBOLS = "Invalid symbols found in provided path";

            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
                return new Error<string>(ERR_EMPTY_PATH);

            for (var i = 0; i < path.Length; i++)
            {
                if (Path.GetInvalidPathChars().Contains(path[i]))
                    return new Error<string>(ERR_INVALID_SYMBOLS);
            }

            try
            {
                _ = Directory.CreateDirectory(path);
                return new Success();
            }
            catch (Exception ex)
            {
                var message = ex.GetFullExceptionErrorMessage();
                return new Error<string>(message);
            }
        }
    }
}
