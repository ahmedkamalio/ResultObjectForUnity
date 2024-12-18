#nullable enable

using System;

namespace ResultObject
{
    /// <summary>
    /// Provides static factory methods for creating Result instances, implementing the Result pattern
    /// for clean and type-safe error handling without exceptions.
    /// </summary>
    /// <remarks>
    /// The Result class serves as the main entry point for creating Result instances in the Result pattern.
    /// It offers convenient factory methods for creating both success and failure results, as well as
    /// utility methods for working with the Unit type and exception handling.
    /// 
    /// This class is designed to work seamlessly with C#'s type inference system and provides
    /// a fluent interface for method chaining.
    /// </remarks>
    /// <example>
    /// Basic usage for success cases:
    /// <code>
    /// // Create a success result with a value
    /// var success = Result.Success(42);
    /// 
    /// // Create a success result with no value using Unit
    /// var unitSuccess = Result.Unit();
    /// </code>
    /// 
    /// Error handling examples:
    /// <code>
    /// // Create a failure result with just a message
    /// var failure = Result.Failure&lt;int&gt;("Operation failed");
    /// 
    /// // Create a failure result with a message and error code
    /// var codedFailure = Result.Failure&lt;string&gt;("Not found", "404");
    /// 
    /// // Using the Try method to handle exceptions
    /// var result = Result.Try(() => ParseInteger("123"));
    /// </code>
    /// </example>
    public static class Result
    {
        /// <summary>
        /// Creates a success Result containing the specified value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be contained in the Result.</typeparam>
        /// <param name="value">The value to be wrapped in a successful Result.</param>
        /// <returns>A new Result instance representing a successful operation containing the specified value.</returns>
        /// <example>
        /// <code>
        /// Result&lt;int&gt; result = Result.Success(42);
        /// result.Match(
        ///     success: value => Console.WriteLine($"Got value: {value}"),
        ///     failure: error => Console.WriteLine($"Error: {error}")
        /// );
        /// </code>
        /// </example>
        public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);

        /// <summary>
        /// Creates a failure Result containing the specified error.
        /// </summary>
        /// <typeparam name="TValue">The type that would have been returned in case of success.</typeparam>
        /// <param name="error">The ResultError instance describing the failure.</param>
        /// <returns>A new Result instance representing a failed operation with the specified error.</returns>
        /// <example>
        /// <code>
        /// var error = new ResultError("Invalid input", "VALIDATION_ERROR");
        /// Result&lt;int&gt; result = Result.Failure&lt;int&gt;(error);
        /// </code>
        /// </example>
        public static Result<TValue> Failure<TValue>(ResultError error) => Result<TValue>.Failure(error);

        /// <summary>
        /// Creates a failure Result with the specified error message and optional error code.
        /// </summary>
        /// <typeparam name="TValue">The type that would have been returned in case of success.</typeparam>
        /// <param name="message">The error message describing what went wrong.</param>
        /// <param name="code">An optional error code for categorization.</param>
        /// <returns>A new Result instance representing a failed operation with the specified error message and code.</returns>
        /// <example>
        /// <code>
        /// Result&lt;User&gt; result = Result.Failure&lt;User&gt;("User not found", "404");
        /// </code>
        /// </example>
        public static Result<TValue> Failure<TValue>(string message, string? code = null) =>
            Result<TValue>.Failure(message, code);

        /// <summary>
        /// Creates a new failure result from an exception.
        /// </summary>
        /// <param name="exception">The exception to convert into a failure result.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing failure.</returns>
        /// <example>
        /// <code>
        /// try 
        /// {
        ///     DoSomethingAsync(); // assuming this will throw
        /// }
        /// catch (Exception ex)
        /// {
        ///     var resultFromException = Result.Failure&lt;MyType&gt;(exception);
        /// }
        /// </code>
        /// </example>
        public static Result<TValue> Failure<TValue>(Exception exception) => Result<TValue>.Failure(exception);

        /// <summary>
        /// Executes the provided function within a try-catch block and wraps the result in a Result instance.
        /// If an exception occurs, it is captured and converted into a failure Result.
        /// </summary>
        /// <typeparam name="TValue">The type of value that the action returns.</typeparam>
        /// <param name="action">The function to execute that may throw an exception.</param>
        /// <returns>
        /// A success Result containing the function's return value if no exception occurs,
        /// or a failure Result containing the captured exception if one is thrown.
        /// </returns>
        /// <example>
        /// Basic Usage:
        /// <code>
        /// Result&lt;int&gt; result = Result.Try(() => 
        /// {
        ///     return int.Parse("not a number");  // This will throw
        /// });
        /// 
        /// result.Match(
        ///     success: value => Console.WriteLine($"Parsed: {value}"),
        ///     failure: error => Console.WriteLine($"Parse failed: {error}")
        /// );
        /// </code>
        /// </example>
        public static Result<TValue> Try<TValue>(Func<TValue> action) => Result<TValue>.Try(action);

        /// <summary>
        /// Creates a success Result containing a Unit value, used when an operation succeeds but returns no meaningful value.
        /// </summary>
        /// <returns>A new Result instance representing a successful operation with no value.</returns>
        /// <example>
        /// <code>
        /// public Result&lt;Unit&gt; SaveChanges()
        /// {
        ///     // Perform save operation
        ///     return Result.Unit();  // Indicate success with no return value
        /// }
        /// </code>
        /// </example>
        public static Result<Unit> Unit() => Result<Unit>.Success(ResultObject.Unit.Value);

        /// <summary>
        /// Creates a failure Result of Unit type with the specified error.
        /// </summary>
        /// <param name="error">The ResultError instance describing the failure.</param>
        /// <returns>A new Result instance representing a failed operation with no value type.</returns>
        /// <example>
        /// <code>
        /// public Result&lt;Unit&gt; SaveChanges()
        /// {
        ///     if (error)
        ///         return Result.Unit(new ResultError("Save failed", "IO_ERROR"));
        ///     return Result.Unit();
        /// }
        /// </code>
        /// </example>
        public static Result<Unit> Unit(ResultError error) => Result<Unit>.Failure(error);
    }
}