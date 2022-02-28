using Ant0nRocket.Lib.Std20.Attributes;
using Ant0nRocket.Lib.Std20.IO;

namespace Ant0nRocket.Lib.Std20.Extensions
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
