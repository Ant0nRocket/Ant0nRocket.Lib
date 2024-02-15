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
        public string FileName { get; private set; }

        /// <summary>
        /// Relative or absolute directory path to <see cref="FileName"/>.
        /// </summary>
        public string DirectoryName { get; private set; }

        /// <summary>
        /// If true - previous version of a file will be saved but
        /// only when using <see cref="FileSystemUtils.TrySaveToFile{T}(T, string?, bool?)"/>.
        /// </summary>
        public bool BackupOldData { get; private set; } = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StoreAttribute(string? fileName = default, string? subDirectory = default, bool backupOldData = false)
        {
            FileName = fileName ?? string.Empty;
            DirectoryName = subDirectory ?? string.Empty;
            BackupOldData = backupOldData;
        }
    }
}
