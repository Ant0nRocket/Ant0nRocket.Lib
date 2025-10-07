using Ant0nRocket.Lib.Extensions;
using System;

namespace Ant0nRocket.Lib.Patterns
{
    /// <summary>
    /// Generic version of <see cref="Result"/>.
    /// Additionally to base class contains a Value property for cases
    /// when you don't simply want to know success or not but when you need a
    /// result of an operation.
    /// <code>
    /// Result&lt;DirectoryInfo&gt; result = FileSystemUtils.TouchDirectory(path);
    /// if (result.IsSuccess) 
    /// { 
    ///     var dirInfo = result.Value;
    /// }
    /// </code>
    /// </summary>
    public record Result<T> : Result
    {
        /// <summary>
        /// Instance of <typeparamref name="T"/> in case of <see cref="Result.IsSuccess"/>
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Non-public to force fabric style, see fabric method below.
        /// </summary>
        protected Result(T? value, bool isSuccess, string? error) : base(isSuccess, error)
        {
            Value = value;
        }

        /// <summary>
        /// Success with instance of <paramref name="value"/> inside.
        /// </summary>
        public static Result<T> Success(T value) => new(value, true, null);

        /// <summary>
        /// Creates a failed result with the specified error message.
        /// </summary>
        public static new Result<T> Failure(string error) => new(default, false, error);

        /// <summary>
        /// Creates a failed result with the specified exception.
        /// </summary>
        public static new Result<T> Failure(Exception ex) => new(default, false, ex.GetFullExceptionErrorMessage());

        #region Implicit operators

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="value">The value to wrap in a successful result</param>
        /// <example>
        /// <code>
        /// // Instead of: Result&lt;User&gt;.Success(user)
        /// // Can write:
        /// Result&lt;User&gt; result = user;
        /// </code>
        /// </example>
        public static implicit operator Result<T>(T value) => Success(value);

        /// <summary>
        /// Implicitly converts an error message to a failed <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="error">The error message for the failed result</param>
        /// <example>
        /// <code>
        /// // Instead of: Result&lt;User&gt;.Failure("Not found")
        /// // Can write:
        /// Result&lt;User&gt; error = "User not found";
        /// </code>
        /// </example>
        public static implicit operator Result<T>(string error) => Failure(error);

        /// <summary>
        /// Implicitly converts an exception to a failed <see cref="Result{T}"/>.
        /// The exception message will be extracted using <see cref="ExceptionExtensions.GetFullExceptionErrorMessage(Exception, string)"/>.
        /// </summary>
        /// <param name="ex">The exception to convert to a failed result</param>
        /// <example>
        /// <code>
        /// // Instead of: Result&lt;User&gt;.Failure(exception)
        /// // Can write:
        /// Result&lt;User&gt; error = new NotFoundException("User not found");
        /// </code>
        /// </example>
        public static implicit operator Result<T>(Exception ex) => Failure(ex);

        #endregion

        #region Pattern matching

        /// <summary>
        /// Enables pattern matching support for Result&lt;T&gt;.
        /// Allows deconstruction into (isSuccess, value, error) tuple.
        /// </summary>
        public void Deconstruct(out bool isSuccess, out T? value, out string? error)
        {
            isSuccess = IsSuccess;
            value = Value;
            error = Error;
        }

        #endregion
    }
}
