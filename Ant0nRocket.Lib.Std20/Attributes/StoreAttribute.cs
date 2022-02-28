using System;

namespace Ant0nRocket.Lib.Std20.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class StoreAttribute : Attribute
    {
        public string FileName { get; private set; }

        public string DirectoryName { get; private set; }

        public bool BackupOldData { get; private set; } = false;

        public StoreAttribute(string fileName = default, string subDirectory = default, bool backupOldData = false)
        {
            FileName = fileName ?? string.Empty;
            DirectoryName = subDirectory ?? string.Empty;
            BackupOldData = backupOldData;
        }
    }
}
