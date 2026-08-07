using Ant0nRocket.Lib.Attributes;
using System;

namespace Ant0nRocket.Lib.Exceptions
{
    /// <summary>
    /// Use when need to highlight then class should be marked with
    /// <see cref="AppDataLocationAttribute"/>
    /// </summary>
    public class MissingAppDataLocationAttributeException(string? message) : Exception(message)
    {
    }
}
