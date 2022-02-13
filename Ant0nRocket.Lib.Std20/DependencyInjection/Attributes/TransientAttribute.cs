using System;

namespace Ant0nRocket.Lib.Std20.DependencyInjection.Attributes
{
    /// <summary>
    /// Attribute class for <see cref="DI"/>. If set then every call of <see cref="DI.Get{T}"/>
    /// will return a new instance of an object.
    /// </summary>
    public class TransientAttribute : Attribute
    {
    }
}
