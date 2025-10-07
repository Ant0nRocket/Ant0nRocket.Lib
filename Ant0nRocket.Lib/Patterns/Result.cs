using Ant0nRocket.Lib.Extensions;
using System;

namespace Ant0nRocket.Lib.Patterns
{
    /// <summary>
    /// Base class for implementing Result Pattern.
    /// <code>
    /// var result = FileSystemUtils.TouchDirectory(path);
    /// if (result.IsSuccess) { /* code here */ }
    /// </code>
    /// </summary>
    public record Result
    {
        /// <summary>
        /// Indicates that something was succeeded.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates that there was an error.
        /// <see cref="Error"/> for more details about the error.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Error message if <see cref="IsFailure"/> was set to true.
        /// </summary>
        public string? Error { get; }

        /// <summary>
        /// Non-public to force fabric style, see fabric method below.
        /// </summary>
        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Simple success result
        /// </summary>
        public static Result Success() => new(true, null);

        /// <summary>
        /// Simple failure result with error message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result Failure(string error) => new(false, error);

        /// <summary>
        /// Same as <see cref="Failure(string)"/> but short-handed for exceptions:
        /// <see cref="ExceptionExtensions.GetFullExceptionErrorMessage(Exception, string)"/> will be
        /// called for retrieve error message.
        /// </summary>
        public static Result Failure(Exception ex) => new(false, ex.GetFullExceptionErrorMessage());
    }
}
