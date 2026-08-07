using Ant0nRocket.Lib.IO;
using System;

namespace Ant0nRocket.Lib.Attributes
{
    /// <summary>
    /// Attribute describes where to store serialized version of a class.
    /// Only <see cref="FileSystemUtils.LoadOrCreateFromAppData{T}"/> and
    /// <see cref="FileSystemUtils.SaveToAppData{T}(T, string?, bool, string, System.Text.Json.JsonSerializerOptions?)"/> use it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AppDataLocationAttribute : Attribute
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
        public AppDataLocationAttribute(string fileName, string subDirectory, string fileNameExt = ".json")
        {
            FileName = fileName ?? string.Empty;
            SubdirectoryName = subDirectory ?? string.Empty;

            /*
             Что тут происходит?
            Легче всего задавать FileName методом nameof(className) и чтобы не
            городить пристыковку расширения файла - передадим его по-умолчанию как
            ".json", но если пользователь всё таки задал какое-то - просто идём дальше.
             */

            if (fileName?.IndexOf('.') >= 0) return; // some extension specified, all done
            FileName += fileNameExt;
        }
    }
}
