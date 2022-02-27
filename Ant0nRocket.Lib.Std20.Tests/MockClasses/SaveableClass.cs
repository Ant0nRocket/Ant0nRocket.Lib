using Ant0nRocket.Lib.Std20.DependencyInjection.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ant0nRocket.Lib.Std20.Tests.MockClasses
{
    [Save(fileName: $"{nameof(SaveableClass)}.dat")]
    public class SaveableClass
    {
        public string Name { get; set; } = nameof(SaveableClass);
    }
}
