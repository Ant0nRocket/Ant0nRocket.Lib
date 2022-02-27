using Ant0nRocket.Lib.Std20.DependencyInjection;
using Ant0nRocket.Lib.Std20.IO;
using Ant0nRocket.Lib.Std20.Tests.MockClasses;
using Ant0nRocket.Lib.Std20.Tests.MockInterfaces;

using NUnit.Framework;

using System.IO;

namespace Ant0nRocket.Lib.Std20.Tests
{
    public class DependencyInjectionTests
    {
        [Test]
        public void T001_TransientAttribute()
        {
            var instA = DI.Get<TransientClass>();
            var instB = DI.Get<TransientClass>();
            Assert.AreNotSame(instB, instA);
        }

        [Test]
        public void T002_SingletonWithNoSaveAttribute()
        {
            var instA = DI.Get<BasicClass>();
            var instB = DI.Get<BasicClass>();
            Assert.AreSame(instB, instA);

            instA.SomeInt = int.MaxValue - 1;
            Assert.AreEqual(int.MaxValue - 1, instB.SomeInt);
        }

        [Test]
        public void T003_SavingToFile()
        {
            var ints = DI.Get<SaveableClass>();
            DI.SaveAll();

            var fileName = FileSystemUtils.GetDefaultAppDataFolderPathFor($"{nameof(SaveableClass)}.dat");
            Assert.IsTrue(File.Exists(fileName));

            var contents = File.ReadAllText(fileName);
            var expectedContents = Newtonsoft.Json.JsonConvert.SerializeObject(ints);
            Assert.AreEqual(expectedContents, contents);
        }

        [Test]
        public void T004_ReadingFromFile()
        {
            var instA = DI.Get<SaveableClass>();
            instA.Name = "TestName";
            DI.Unload<SaveableClass>(saveBeforeUnload: true);

            var instB = DI.Get<SaveableClass>();
            Assert.AreNotSame(instB, instA);
            Assert.AreEqual(instA.Name, instB.Name);
        }

        [Test]
        public void T005_Interfaces()
        {
            DI.Register<IMockInterface, MockInterfacedClass>();
            var inst = DI.Get<IMockInterface>();
            Assert.IsNotNull(inst);
        }
    }
}
