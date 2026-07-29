using Ant0nRocket.Lib.Attributes;
using Ant0nRocket.Lib.Configuration;
using Ant0nRocket.Lib.Helpers;
using Ant0nRocket.Lib.IO;
using Ant0nRocket.Lib.Patterns;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace Ant0nRocket.Lib.Tests.IO
{
    [TestFixture]
    public class FileSystemUtilsTests
    {
        private string _testRoot;
        private string _dataDirectory;

        // Тестовый класс с атрибутом Store
        [Store("test.json", "TestDir")]
        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            // Сбрасываем форсирование (чтобы тесты были независимы)
            SetPortableMode(null);

            // Устанавливаем имя приложения для тестов
            SetApplicationName("TestApp");

            // Создаём временную корневую папку для тестов, работающих с файлами
            _testRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testRoot);

            // Папка Data для портативного режима (создаётся, если нужно)
            _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
            Directory.CreateDirectory(_dataDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            // Сбрасываем форсирование, чтобы не влиять на другие тесты
            SetPortableMode(null);

            // Удаляем временную папку
            if (Directory.Exists(_testRoot))
                Directory.Delete(_testRoot, true);

            // Удаляем папку Data из BaseDirectory, если она была создана тестами
            if (Directory.Exists(_dataDirectory))
                Directory.Delete(_dataDirectory, true);
        }

        #region Вспомогательные методы управления окружением

        /// <summary>
        /// Устанавливает режим портативности через приватное свойство ForcePortableMode.
        /// Если передать null, форсирование снимается.
        /// </summary>
        private void SetPortableMode(bool? portable)
        {
            var type = typeof(EnvironmentHelper);
            var prop = type.GetProperty("ForcePortableMode", BindingFlags.Static | BindingFlags.NonPublic);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(null, portable);
                return;
            }
            // Fallback: если свойство не найдено, попробуем поле
            var field = type.GetField("ForcePortableMode", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, portable);
            }
        }

        /// <summary>
        /// Устанавливает имя приложения для корректного пути в GetDataDirectoryName.
        /// </summary>
        private void SetApplicationName(string name)
        {
            var type = typeof(ApplicationInfo);
            var prop = type.GetProperty("ApplicationName", BindingFlags.Static | BindingFlags.Public);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(null, name);
                return;
            }
            var field = type.GetField("_applicationName", BindingFlags.Static | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(null, name);
            }
        }

        #endregion

        #region Тесты для CanWriteToDirectory / CanWriteToBaseDirectory

        [Test]
        public void CanWriteToDirectory_WithExistingDirectory_ReturnsTrue()
        {
            Assert.That(FileSystemUtils.CanWriteToDirectory(Path.GetTempPath()), Is.True);
        }

        [Test]
        public void CanWriteToDirectory_WithNonExistentDirectory_ReturnsFalse()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Assert.That(FileSystemUtils.CanWriteToDirectory(path), Is.False);
        }

        [Test]
        public void CanWriteToBaseDirectory_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => FileSystemUtils.CanWriteToBaseDirectory());
        }

        #endregion

        #region Тесты для GetDataDirectoryName

        [Test]
        public void GetDataDirectoryName_PortableMode_ReturnsBaseDirectoryPlusData()
        {
            SetPortableMode(true);
            var expected = Path.Combine(AppContext.BaseDirectory, "Data");
            Assert.That(FileSystemUtils.GetDataDirectoryName(), Is.EqualTo(expected));
            Assert.That(FileSystemUtils.GetDataDirectoryName(true), Is.EqualTo(expected));
        }

        [Test]
        public void GetDataDirectoryName_InstalledMode_LocalData()
        {
            SetPortableMode(false);
            var expected = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ApplicationInfo.ApplicationName);
            Assert.That(FileSystemUtils.GetDataDirectoryName(false), Is.EqualTo(expected));
        }

        [Test]
        public void GetDataDirectoryName_InstalledMode_RoamingData()
        {
            SetPortableMode(false);
            var expected = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                ApplicationInfo.ApplicationName);
            Assert.That(FileSystemUtils.GetDataDirectoryName(true), Is.EqualTo(expected));
        }

        #endregion

        #region Тесты для GetFilePathParts

        [TestCase(@"C:\folder\file.txt", @"C:\folder", "file", ".txt")]
        [TestCase(@"file.txt", "", "file", ".txt")]
        [TestCase(@"C:\folder\file", @"C:\folder", "file", "")]
        [TestCase(@"C:\folder\", @"C:\folder", "", "")]
        public void GetFilePathParts_ReturnsCorrectParts(string fullPath, string dir, string nameWithoutExt, string ext)
        {
            var (directoryName, fileNameWithoutExt, extension) = FileSystemUtils.GetFilePathParts(fullPath);
            Assert.That(directoryName, Is.EqualTo(dir));
            Assert.That(fileNameWithoutExt, Is.EqualTo(nameWithoutExt));
            Assert.That(extension, Is.EqualTo(ext));
        }

        #endregion

        #region Тесты для TouchDirectory

        [Test]
        public void TouchDirectory_CreatesNewDirectory_ReturnsSuccess()
        {
            var path = Path.Combine(_testRoot, "NewDir");
            var result = FileSystemUtils.TouchDirectory(path);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(Directory.Exists(path), Is.True);
        }

        [Test]
        public void TouchDirectory_WhenDirectoryExists_ReturnsSuccess()
        {
            var path = Path.Combine(_testRoot, "ExistingDir");
            Directory.CreateDirectory(path);
            var result = FileSystemUtils.TouchDirectory(path);
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void TouchDirectory_WithNullPath_ReturnsFailure()
        {
            var result = FileSystemUtils.TouchDirectory(null);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Does.Contain("Null path"));
        }

        [Test]
        public void TouchDirectory_WithEmptyPath_ReturnsFailure()
        {
            var result = FileSystemUtils.TouchDirectory(string.Empty);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Does.Contain("Null path"));
        }

        #endregion

        #region Тесты для ScanDirectoryRecursively

        [Test]
        public void ScanDirectoryRecursively_CallsActionForEachFile()
        {
            var root = Path.Combine(_testRoot, "ScanRoot");
            Directory.CreateDirectory(root);
            File.WriteAllText(Path.Combine(root, "file1.txt"), "");
            File.WriteAllText(Path.Combine(root, "file2.txt"), "");
            var sub = Path.Combine(root, "Sub");
            Directory.CreateDirectory(sub);
            File.WriteAllText(Path.Combine(sub, "file3.txt"), "");

            var found = new System.Collections.Generic.List<string>();
            FileSystemUtils.ScanDirectoryRecursively(root, file => found.Add(file));

            Assert.That(found.Count, Is.EqualTo(3));
            Assert.That(found, Does.Contain(Path.Combine(root, "file1.txt")));
            Assert.That(found, Does.Contain(Path.Combine(root, "file2.txt")));
            Assert.That(found, Does.Contain(Path.Combine(sub, "file3.txt")));
        }

        [Test]
        public void ScanDirectoryRecursively_WithNullAction_DoesNothing()
        {
            var path = Path.Combine(_testRoot, "ScanRoot");
            Directory.CreateDirectory(path);
            Assert.DoesNotThrow(() => FileSystemUtils.ScanDirectoryRecursively(path, null));
        }

        [Test]
        public void ScanDirectoryRecursively_WithNonExistingPath_DoesNothing()
        {
            var called = false;
            FileSystemUtils.ScanDirectoryRecursively(Path.Combine(_testRoot, "NonExisting"), _ => called = true);
            Assert.That(called, Is.False);
        }

        #endregion

        #region Тесты для Delete

        [Test]
        public void Delete_File_ReturnsTrueAndDeletes()
        {
            var file = Path.Combine(_testRoot, "delete.txt");
            File.WriteAllText(file, "test");
            Assert.That(FileSystemUtils.Delete(file), Is.True);
            Assert.That(File.Exists(file), Is.False);
        }

        [Test]
        public void Delete_Directory_ReturnsTrueAndDeletesRecursively()
        {
            var dir = Path.Combine(_testRoot, "DeleteDir");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, "file.txt"), "");
            Assert.That(FileSystemUtils.Delete(dir), Is.True);
            Assert.That(Directory.Exists(dir), Is.False);
        }

        [Test]
        public void Delete_NonExistingFile_ReturnsFalse()
        {
            var path = Path.Combine(_testRoot, "nonexistent.txt");
            Assert.That(FileSystemUtils.Delete(path), Is.False);
        }

        #endregion

        #region Тесты для SaveFileToData и ReadFileFromData

        [Test]
        public void SaveFileToData_SavesFileAndCreatesBackup()
        {
            // Убедимся, что мы в портативном режиме для предсказуемости
            SetPortableMode(true);

            var data = new TestData { Id = 1, Name = "Test" };

            // Первое сохранение
            var result1 = FileSystemUtils.SaveFileToData(data);
            Assert.That(result1.IsSuccess, Is.True);

            // Проверяем, что файл создан
            var expectedPath = Path.Combine(FileSystemUtils.GetDataDirectoryName(), "TestDir", "test.json");
            Assert.That(File.Exists(expectedPath), Is.True);

            // Второе сохранение с бэкапом
            var result2 = FileSystemUtils.SaveFileToData(data);
            Assert.That(result2.IsSuccess, Is.True);

            // Проверяем наличие бэкапа
            var backupDir = Path.Combine(Path.GetDirectoryName(expectedPath), "Backup");
            Assert.That(Directory.Exists(backupDir), Is.True);
            var backupFiles = Directory.GetFiles(backupDir, "test_*.json");
            Assert.That(backupFiles.Length, Is.EqualTo(1));
        }

        [Test]
        public void ReadFileFromData_WhenFileExists_ReturnsSuccessWithDeserializedObject()
        {
            SetPortableMode(true);
            var data = new TestData { Id = 42, Name = "ReadTest" };
            FileSystemUtils.SaveFileToData(data);

            var readResult = FileSystemUtils.ReadFileFromDataOrNew<TestData>();
            Assert.That(readResult.IsSuccess, Is.True);
            Assert.That(readResult.Value.Id, Is.EqualTo(42));
            Assert.That(readResult.Value.Name, Is.EqualTo("ReadTest"));
        }

        [Test]
        public void ReadFileFromData_WhenFileDoesNotExist_ReturnsFailure()
        {
            SetPortableMode(true);
            // Удаляем файл, если он существует
            var expectedPath = Path.Combine(FileSystemUtils.GetDataDirectoryName(), "TestDir", "test.json");
            if (File.Exists(expectedPath))
                File.Delete(expectedPath);

            var result = FileSystemUtils.ReadFileFromDataOrNew<TestData>();
            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public void SaveFileToData_WithCustomPath_OverridesAttribute()
        {
            SetPortableMode(true);
            var data = new TestData { Id = 3, Name = "Custom" };
            var customPath = Path.Combine(_testRoot, "custom.json");
            var result = FileSystemUtils.SaveFileToData(data, destFilePath: customPath);
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(File.Exists(customPath), Is.True);

            var json = File.ReadAllText(customPath);
            var deserialized = JsonSerializer.Deserialize<TestData>(json);
            Assert.That(deserialized.Id, Is.EqualTo(3));
        }

        [Test]
        public void SaveFileToData_WithBackupDisabled_DoesNotCreateBackup()
        {
            SetPortableMode(true);
            var data = new TestData { Id = 1, Name = "NoBackup" };
            FileSystemUtils.SaveFileToData(data); // первый раз
            var result = FileSystemUtils.SaveFileToData(data, backupOldData: false);
            Assert.That(result.IsSuccess, Is.True);

            var backupDir = Path.Combine(FileSystemUtils.GetDataDirectoryName(), "TestDir", "Backup");
            Assert.That(Directory.Exists(backupDir), Is.False);
        }

        #endregion
    }
}