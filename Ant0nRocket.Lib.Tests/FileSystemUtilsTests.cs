using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.IO;
using Ant0nRocket.Lib.Reflection;
using Ant0nRocket.Lib.Tests.MockClasses;
using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ant0nRocket.Lib.Tests
{
    public class FileSystemUtilsTests : _TestsBase
    {
        [Test]
        public void T001_AppName()
        {
            var appName = ReflectionUtils.AppName;
            Assert.AreEqual(appName, "testhost");
        }

        [Test]
        public void T002_IsPortable()
        {
            var isPortable = Ant0nRocketLibConfig.IsPortableMode;
            Assert.AreEqual(isPortable, false);
        }

        [Test]
        public void T003_DefaultAppDataFolderPath()
        {
            var defaultAppDataFolder = FileSystemUtils.DefaultSpecialFolder;
            Assert.AreEqual(Environment.SpecialFolder.LocalApplicationData, defaultAppDataFolder);
        }

        [Test]
        public void T004_AppDataLocalPath()
        {
            var appDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var calculatedAppDataLocalPath = Path.GetDirectoryName(FileSystemUtils.GetAppDataLocalFolderPath()); // ".."
            Assert.AreEqual(appDataLocalPath, calculatedAppDataLocalPath);
        }

        [Test]
        public void T005_AppDataRoamingPath()
        {
            var appDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var calculatedAppDataLocalPath = Path.GetDirectoryName(FileSystemUtils.GetAppDataRoamingFolderPath()); // ".."
            Assert.AreEqual(appDataLocalPath, calculatedAppDataLocalPath);
        }

        [Test]
        public void T005_GetAppDataPathForTestsArePortable()
        {
            Ant0nRocketLibConfig.IsPortableMode = true;

            const string FILENAME = "somefile.dat";
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            rootPath = Path.Combine(rootPath, FILENAME);
            var libResult = FileSystemUtils.GetDefaultAppDataFolderPathFor(fileName: FILENAME);

            Assert.AreEqual(rootPath, libResult);
        }

        [Test]
        public void T006_GetAppDataPathForTestsArePortableWithSubDirectory()
        {
            Ant0nRocketLibConfig.IsPortableMode = true;

            const string FILENAME = "somefile.dat";
            const string SUBDIRECTORY = "Data";

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            rootPath = Path.Combine(rootPath, SUBDIRECTORY, FILENAME);
            var libResult = FileSystemUtils.GetDefaultAppDataFolderPathFor(fileName: FILENAME, subDirectory: SUBDIRECTORY);

            Assert.AreEqual(rootPath, libResult);
        }

        [Test]
        public void T007_GetAppDataPathForTestsAreNotPortable()
        {
            Ant0nRocketLibConfig.IsPortableMode = false;

            const string FILENAME = "somefile.dat";
            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var assemblyName = Assembly.GetEntryAssembly()?.GetName()?.Name ?? string.Empty;
            rootPath = Path.Combine(rootPath, assemblyName, FILENAME);

            var libResult = FileSystemUtils.GetDefaultAppDataFolderPathFor(fileName: FILENAME);

            Assert.AreEqual(rootPath, libResult);
        }

        [Test]
        public void T007_SetPortableToTrueForTests()
        {
            Ant0nRocketLibConfig.IsPortableMode = true;
        }

        [Test]
        public void T008_ReadingClassFromFile()
        {
            var storeAttr = ReflectionUtils.GetAttribute<StoreAttribute>(typeof(StoreClass));
            var filePath = storeAttr!.GetDefaultAppDataFolderPath(true);
            if (File.Exists(filePath)) File.Delete(filePath);
            File.WriteAllText(filePath, "{\"TestString\":\"Hello world!\"}");

            var instance = FileSystemUtils.TryReadFromFile<StoreClass>(createInstanceOnError: false);
            Assert.IsNotNull(instance);
            Assert.AreEqual("Hello world!", instance!.TestString);
        }

        [Test]
        public void T009_SavingClassToFile()
        {
            var instance = new StoreClass();
            instance.TestString = "Hello world!";
            FileSystemUtils.TrySaveToFile(instance);

            var storeAttr = ReflectionUtils.GetAttribute<StoreAttribute>(typeof(StoreClass));
            var filePath = storeAttr!.GetDefaultAppDataFolderPath(true);
            var contents = File.ReadAllText(filePath);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<StoreClass>(contents);

            Assert.IsNotNull(obj);
            Assert.AreEqual("Hello world!", obj?.TestString);
        }

        [Test]
        public void T010_SavingClassToFileInNonExistingFolder()
        {
            var instance = new StoreClass { TestString = "Test string" };
            var savePath = @"Some folder/Another one/And another/file.json";
            var saveResult = FileSystemUtils.TrySaveToFile(instance, savePath);
            Assert.That(saveResult is true);
            Assert.That(File.Exists(savePath));
            Assert.That(FileSystemUtils.Delete(savePath));
        }

        [Test]
        public void T011_ScanDirectoryRecursively()
        {
            var rootPath = Path.Combine(FileSystemUtils.GetDefaultAppDataFolderPath(), "scanTest");
            const string D1_NAME = "d1";
            const string D2_NAME = "d2";
            const string D3_NAME = "d3";
            var d1Path = Path.Combine(rootPath, D1_NAME);
            var d2Path = Path.Combine(d1Path, D2_NAME);
            var d3Path = Path.Combine(d2Path, D3_NAME);

            FileSystemUtils.TouchDirectory(d1Path);
            FileSystemUtils.TouchDirectory(d2Path);
            FileSystemUtils.TouchDirectory(d3Path);

            File.Create(Path.Combine(d1Path, "test.file")).Close();
            File.Create(Path.Combine(d2Path, "test.file")).Close();
            File.Create(Path.Combine(d3Path, "test.file")).Close();

            var filesList = new List<string>();
            FileSystemUtils.ScanDirectoryRecursively(rootPath, f => filesList.Add(f));

            Assert.AreEqual(3, filesList.Count);

            filesList.ForEach(f => Debug.WriteLine(f));
            filesList.ForEach(f => File.Delete(f));
            Directory.Delete(rootPath, true);
        }
    }
}
