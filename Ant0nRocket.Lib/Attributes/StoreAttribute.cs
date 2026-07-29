using Ant0nRocket.Lib.IO;
using System;

namespace Ant0nRocket.Lib.Attributes
{
    /// <summary>
    /// Attribute describes where to store serialized version of a class.
    /// Only <see cref="FileSystemUtils.TryReadFromFile{T}(string?, bool)"/> and
    /// <see cref="FileSystemUtils.TrySaveToFile{T}(T, string?, bool?)"/> use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StoreAttribute : Attribute
    {
        /// <summary>
        /// File name without path part.
        /// <code>
        /// somefile.json
        /// another_file.ext
        /// yeat-another.dat
        /// </code>
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Relative (see <see cref="FileSystemUtils.GetDataDirectoryName(bool)"/>) directory path to <see cref="FileName"/>.
        /// </summary>
        public string SubdirectoryName { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StoreAttribute(string fileName, string subDirectory)
        {
            FileName = fileName ?? string.Empty;
            SubdirectoryName = subDirectory ?? string.Empty;
        }
    }
}
