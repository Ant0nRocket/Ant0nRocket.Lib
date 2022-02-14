using Ant0nRocket.Lib.Std20.IO;

using NUnit.Framework;

using System;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class FileSystemUtilsTests
    {
        [Test]
        public void T001_AppName()
        {
            var appName = FileSystemUtils.AppName;
            Assert.AreEqual(appName, "testhost");
        }

        [Test]
        public void T002_IsPortable()
        {
            var isPortable = FileSystemUtils.IsPortableMode;
            Assert.AreEqual(isPortable, true);
        }

        [Test]
        public void T003_DefaultAppDataFolderPath()
        {
            var defaultAppDataFolder = FileSystemUtils.DefaultAppDataFolder;
            Assert.AreEqual(Environment.SpecialFolder.LocalApplicationData, defaultAppDataFolder);
        }
    }
}
