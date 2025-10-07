using System;
using System.Collections.Generic;

namespace Ant0nRocket.Lib.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Exception"/>.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Exceptions could contain a chain of inner exceptions. It's not always easy to
        /// understand what's going on looking on first exception message. This extension method
        /// will get all error messages into single line like:
        /// <i>Error 1</i> -> <i>Error 2</i> -> <i>etc.</i>.
        /// You can change separator.
        /// </summary>
        public static string GetFullExceptionErrorMessage(this Exception? exception, string messagesSeparator = " -> ", bool includeExceptionType = false)
        {
            if (exception == null)
                return string.Empty;

            var messages = new List<string>();
            var currentException = exception; // fast, just a link to exception

            while (currentException != null) // checked
            {
                var message = includeExceptionType
                    ? $"[{currentException.GetType().Name}] {currentException.Message}"
                    : currentException.Message;

                messages.Add(message);
                currentException = currentException.InnerException;
            }

            return string.Join(messagesSeparator, messages);
        }
    }
}
