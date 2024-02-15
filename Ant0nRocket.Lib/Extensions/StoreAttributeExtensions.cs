using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.IO;

namespace Ant0nRocket.Lib.Extensions
{
    public static class StoreAttributeExtensions
    {
        public static string GetDefaultAppDataFolderPath(
            this StoreAttribute storeAttribute,
            bool autoTouchDirectory = false) => ///// <---------
            FileSystemUtils.GetDefaultAppDataFolderPathFor(
                storeAttribute.FileName,
                storeAttribute.DirectoryName,
                autoTouchDirectory: autoTouchDirectory);
    }
}
