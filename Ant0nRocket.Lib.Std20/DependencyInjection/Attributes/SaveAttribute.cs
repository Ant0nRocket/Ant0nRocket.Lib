using System;
using System.Text.RegularExpressions;

namespace Ant0nRocket.Lib.Std20.DependencyInjection.Attributes
{
    public class SaveAttribute : Attribute
    {
        private string fileName = string.Empty;

        /// <summary>
        /// When set this value all special chars (like "!", "@", "&", etc.)
        /// will be replaced with nothing. Only discards allowed ("_").
        /// </summary>
        public string FileName
        {
            get => fileName;
            set => fileName = Regex.Replace(value, @"[^\w\.]", string.Empty);
        }

        private string directoryName = default;

        /// <summary>
        /// When set this value all special chars (like "!", "@", "&", etc.)
        /// will be replaced with nothing. Only discards allowed ("_").
        /// </summary>
        public string DirectoryName
        {
            get => directoryName;
            set => directoryName = Regex.Replace(value, @"[^\w\.]", string.Empty);
        }

        /// <summary>
        /// Indicates that previous version should be saved before writing a new one.
        /// </summary>
        public bool BackupOldData { get; set; } = false;


        public SaveAttribute(string fileName, string directoryName = default, bool backupOldData = false)
        {
            DirectoryName = directoryName;
            FileName = fileName;
            BackupOldData = backupOldData;
        }
    }
}
