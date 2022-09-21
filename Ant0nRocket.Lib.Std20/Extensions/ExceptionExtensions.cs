using System;
using System.Collections.Generic;

namespace Ant0nRocket.Lib.Std20.Extensions
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
        public static string GetFullExceptionErrorMessage(this Exception exception, string messagesSeparator = " -> ")
        {
            List<string> errorList = new();

            void AppendErrorListRecursevely(Exception ex)
            {
                errorList.Add(ex.Message);
                if (ex.InnerException != null)
                    AppendErrorListRecursevely(ex.InnerException);
            }

            if (exception == null)
                return string.Empty;

            AppendErrorListRecursevely(exception);

            return string.Join(messagesSeparator, errorList);
        }
    }
}
