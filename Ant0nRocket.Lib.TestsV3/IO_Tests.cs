using Ant0nRocket.Lib.IO.FileSystem;
using OneOf.Types;
using System.Runtime.Versioning;

namespace Ant0nRocket.Lib.TestsV6
{
    public class IO_Tests
    {
        [Test]
        public void T001_FileSystem_TouchDirectory_Null()
        {
            var t = FileSystem.TouchDirectory(null);
            Assert.That(t.Value is Error<string>);
        }

        [Test]
        public void T002_FileSystem_TouchDirectory_InvalidSymbol()
        {
            var t = FileSystem.TouchDirectory(@"|/../.."); // '|' is forbidden
            Assert.That(t.Value is Error<string>);
        }

        [Test]
        [SupportedOSPlatform("windows")]
        public void T003_FileSystem_TouchDirectory_AccessDenied()
        {
            // �� �������
            var t = FileSystem.TouchDirectory("C:/Windows/_Dir");
            Assert.That(t.Value is Error<string>);
        }

        [Test]
        public void T004_FileSystem_TouchDirectory_Success()
        {
            var t = FileSystem.TouchDirectory("TestFolder");
            Assert.That(t.Value is Success);
        }
    }
}