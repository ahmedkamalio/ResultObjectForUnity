#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;

namespace ResultObject
{
    /// <summary>
    /// Represents a discriminated union that encapsulates the result of an operation,
    /// containing either a success value of type <typeparamref name="TValue"/> or an error.
    /// This class provides a type-safe way to handle operation outcomes without throwing exceptions.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in case of success.</typeparam>
    /// <remarks>
    /// <para>
    /// This class is immutable and thread-safe. All instance members are safe for
    /// concurrent access by multiple threads.
    /// </para>
    ///
    /// <para>
    /// The Result class is an implementation of the Result pattern, offering a robust way to handle
    /// operation outcomes by explicitly representing both success and failure cases. It promotes:
    /// <list type="bullet">
    /// <item>Type-safe error handling without exceptions</item>
    /// <item>Explicit handling of both success and failure cases</item>
    /// <item>Composable operations through functional programming patterns</item>
    /// <item>Clean and maintainable error propagation</item>
    /// </list>
    /// 
    /// The class provides two nested types:
    /// <list type="bullet">
    /// <item><see cref="SuccessResult"/>: Represents a successful operation with a value</item>
    /// <item><see cref="FailureResult"/>: Represents a failed operation with an error</item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// Basic usage:
    /// <code>
    /// public Result&lt;int&gt; Divide(int numerator, int denominator)
    /// {
    ///     if (denominator == 0)
    ///         return Result&lt;int&gt;.Failure("Division by zero");
    ///     return Result&lt;int&gt;.Success(numerator / denominator);
    /// }
    /// 
    /// var result = Divide(10, 2);
    /// result.Match(
    ///     success: value => Console.WriteLine($"Result: {value}"),
    ///     failure: error => Console.WriteLine($"Error: {error}")
    /// );
    /// </code>
    /// 
    /// Using pattern matching and deconstruction:
    /// <code>
    /// var (isSuccess, value, error) = result;
    /// if (isSuccess)
    ///     Console.WriteLine($"Got value: {value}");
    /// else
    ///     Console.WriteLine($"Got error: {error}");
    /// </code>
    /// </example>
    public abstract class Result<TValue> : IResult<TValue>
    {
        /// <summary>
        /// Gets a value indicating whether the result represents a successful operation.
        /// </summary>
        /// <value>true if the result is a <see cref="SuccessResult"/>; otherwise, false.</value>
        public bool IsSuccess => this is SuccessResult;

        /// <summary>
        /// Gets a value indicating whether the result represents a failed operation.
        /// </summary>
        /// <value>true if the result is a <see cref="FailureResult"/>; otherwise, false.</value>
        public bool IsFailure => this is FailureResult;

        /// <summary>
        /// Creates a new success result containing the specified value.
        /// </summary>
        /// <param name="value">The value to wrap in a success result.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing success.</returns>
        public static Result<TValue> Success(TValue value)
        {
            return new SuccessResult(value);
        }

        /// <summary>
        /// Creates a new failure result with the specified error.
        /// </summary>
        /// <param name="error">The error describing the failure.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing failure.</returns>
        public static Result<TValue> Failure(ResultError error)
        {
            return new FailureResult(error);
        }

        /// <summary>
        /// Creates a new failure result with the specified error message and optional error code.
        /// </summary>
        /// <param name="message">The error message describing what went wrong.</param>
        /// <param name="code">An optional error code for categorization.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing failure.</returns>
        public static Result<TValue> Failure(string message, string? code = null)
        {
            return new FailureResult(new ResultError(message, code));
        }

        /// <summary>
        /// Creates a new failure result from an exception.
        /// </summary>
        /// <param name="exception">The exception to convert into a failure result.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing failure.</returns>
        public static Result<TValue> Failure(Exception exception)
        {
            return new FailureResult(new ResultError(exception));
        }

        /// <summary>
        /// Executes the provided function within a try-catch block and wraps the result.
        /// If an exception occurs, it is captured and converted into a failure result.
        /// </summary>
        /// <remarks>
        /// This method catches all exceptions derived from System.Exception.
        /// Consider catching specific exceptions instead when the error cases are known.
        /// </remarks>
        /// <param name="action">The function to execute that may throw an exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when action is null.</exception>
        /// <returns>
        /// A success result containing the function's return value if no exception occurs,
        /// or a failure result containing the captured exception if one is thrown.
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
        public static Result<TValue> Try(Func<TValue> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                return Success(action());
            }
            catch (Exception ex)
            {
                return Failure(ex);
            }
        }

        /// <summary>
        /// Attempts to get the result value, indicating whether the operation succeeded.
        /// </summary>
        /// <param name="value">When this method returns true, contains the actual value from the successful result.
        /// When this method returns false, contains the default value for TValue.</param>
        /// <returns>true if the result represents success; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// if (result.TryGetValue(out var value))
        ///     Console.WriteLine($"Value: {value}");
        /// </code>
        /// </example>
        public bool TryGetValue(out TValue value)
        {
            return TryGetValueInternal(out value);
        }

        /// <summary>
        /// Attempts to get the error value, indicating whether the operation failed.
        /// </summary>
        /// <param name="error">When this method returns, contains the error from the failed result,
        /// or the default ResultError if the result represents success.</param>
        /// <returns>true if the result represents failure; otherwise, false.</returns>
        /// <example>
        /// <code>
        /// if (result.TryGetError(out var error))
        ///     Console.WriteLine($"Error: {error}");
        /// </code>
        /// </example>
        public bool TryGetError(out ResultError error)
        {
            return TryGetErrorInternal(out error);
        }

        /// <summary>
        /// Maps the success value to a new value using the provided mapping function.
        /// If the result represents failure, the error is propagated to the new result.
        /// </summary>
        /// <remarks>
        /// Map operations create new Result instances and do not modify the original.
        /// For performance-critical scenarios with many transformations, consider
        /// combining multiple map operations.
        /// </remarks>
        /// <typeparam name="TNew">The type of the mapped value.</typeparam>
        /// <param name="mapper">A function that maps the success value to a new value.</param>
        /// <returns>A new result containing either the mapped value or the propagated error.</returns>
        /// <exception cref="ArgumentNullException">Thrown when mapper is null.</exception>
        /// <example>
        /// <code>
        /// var stringResult = integerResult.Map(x => x.ToString());
        /// </code>
        /// </example>
        public Result<TNew> Map<TNew>(Func<TValue, TNew> mapper)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            return Match(value => Result<TNew>.Success(mapper(value)), Result<TNew>.Failure);
        }

        /// <summary>
        /// Chains together multiple operations that return results.
        /// If the current result is successful, applies the binding function to create a new result.
        /// If the current result represents failure, the error is propagated.
        /// </summary>
        /// <typeparam name="TNew">The type of the value in the new result.</typeparam>
        /// <param name="binder">A function that takes the success value and returns a new result.</param>
        /// <returns>The result of the binding function or a failure containing the propagated error.</returns>
        /// <exception cref="ArgumentNullException">Thrown when binder is null.</exception>
        /// <example>
        /// <code>
        /// var finalResult = GetUser()
        ///     .Bind(user => UpdateUser(user))
        ///     .Bind(user => SaveUser(user));
        /// </code>
        /// </example>
        public Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder)
        {
            if (binder is null)
            {
                throw new ArgumentNullException(nameof(binder));
            }

            return Match(binder, Result<TNew>.Failure);
        }

        /// <summary>
        /// Executes the provided action if the result represents success.
        /// </summary>
        /// <param name="action">The action to execute with the success value.</param>
        /// <returns>The same result instance to enable method chaining.</returns>
        /// <example>
        /// <code>
        /// result.OnSuccess(value => Console.WriteLine($"Got value: {value}"))
        ///       .OnFailure(error => Console.WriteLine($"Error: {error}"));
        /// </code>
        /// </example>
        public Result<TValue> OnSuccess(Action<TValue> action)
        {
            if (this is SuccessResult success)
            {
                action(success.Value);
            }

            return this;
        }

        /// <summary>
        /// Executes the provided action if the result represents failure.
        /// </summary>
        /// <param name="action">The action to execute with the error value.</param>
        /// <returns>The same result instance to enable method chaining.</returns>
        /// <example>
        /// <code>
        /// result.OnFailure(error => LogError(error))
        ///       .OnSuccess(value => ProcessValue(value));
        /// </code>
        /// </example>
        public Result<TValue> OnFailure(Action<ResultError> action)
        {
            if (this is FailureResult failure)
            {
                action(failure.Error);
            }

            return this;
        }

        /// <summary>
        /// Pattern matches on the result, executing one of two functions depending on
        /// whether the result represents success or failure.
        /// </summary>
        /// <typeparam name="TResult">The type of the value returned by the pattern matching functions.</typeparam>
        /// <param name="success">The function to execute if the result represents success.</param>
        /// <param name="failure">The function to execute if the result represents failure.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when success is null and the result represents success;
        /// OR when failure is null and the result represents failure.
        /// </exception>
        /// <returns>The value returned by either the success or failure function.</returns>
        /// <example>
        /// <code>
        /// var message = result.Match(
        ///     success: value => $"Success: {value}",
        ///     failure: error => $"Error: {error}"
        /// );
        /// </code>
        /// </example>
        public TResult Match<TResult>(Func<TValue, TResult> success, Func<ResultError, TResult> failure)
        {
            return MatchInternal(success, failure);
        }

        /// <summary>
        /// Deconstructs the result into its components for use with C# deconstruction syntax.
        /// </summary>
        /// <param name="isSuccess">When this method returns, indicates whether the result represents success.</param>
        /// <param name="value">When this method returns, contains the success value or default.</param>
        /// <param name="error">When this method returns, contains the error value or default.</param>
        /// <example>
        /// <code>
        /// var (isSuccess, value, error) = result;
        /// if (isSuccess)
        ///     Console.WriteLine($"Got value: {value}");
        /// else
        ///     Console.WriteLine($"Got error: {error}");
        /// </code>
        /// </example>
        public void Deconstruct(out bool isSuccess, out TValue? value, out ResultError error)
        {
            isSuccess = IsSuccess;
            TryGetValue(out value);
            TryGetError(out error);
        }

        /// <summary>
        /// Internal implementation of pattern matching logic for the specific result type.
        /// </summary>
        /// <typeparam name="TResult">The type of value returned by the pattern matching functions.</typeparam>
        /// <param name="success">The function to execute if the result represents success.</param>
        /// <param name="failure">The function to execute if the result represents failure.</param>
        /// <returns>The value returned by either the success or failure function.</returns>
        /// <remarks>
        /// This protected abstract method is implemented by <see cref="SuccessResult"/> and <see cref="FailureResult"/>
        /// to provide their specific pattern matching behavior. The public <see cref="Match{TResult}"/> method
        /// delegates to this implementation.
        /// </remarks>
        protected abstract TResult MatchInternal<TResult>(Func<TValue, TResult> success,
            Func<ResultError, TResult> failure);

        /// <summary>
        /// Internal implementation of value retrieval logic for the specific result type.
        /// </summary>
        /// <param name="value">When this method returns, contains the value if the result represents success,
        /// or the default value of <typeparamref name="TValue"/> if the result represents failure.</param>
        /// <returns>true if the result represents success and the value was retrieved; otherwise, false.</returns>
        /// <remarks>
        /// This protected abstract method is implemented by <see cref="SuccessResult"/> and <see cref="FailureResult"/>
        /// to provide their specific value retrieval behavior. The public <see cref="TryGetValue"/> method
        /// delegates to this implementation.
        /// </remarks>
        protected abstract bool TryGetValueInternal(out TValue value);

        /// <summary>
        /// Internal implementation of error retrieval logic for the specific result type.
        /// </summary>
        /// <param name="error">When this method returns, contains the error if the result represents failure,
        /// or the default <see cref="ResultError"/> value if the result represents success.</param>
        /// <returns>true if the result represents failure and the error was retrieved; otherwise, false.</returns>
        /// <remarks>
        /// This protected abstract method is implemented by <see cref="SuccessResult"/> and <see cref="FailureResult"/>
        /// to provide their specific error retrieval behavior. The public <see cref="TryGetError"/> method
        /// delegates to this implementation.
        /// </remarks>
        protected abstract bool TryGetErrorInternal(out ResultError error);

        /// <summary>
        /// Represents a successful result containing a value of type <c>TValue</c>.
        /// This class provides a concrete implementation for successful operations in the Result pattern.
        /// </summary>
        /// <remarks>
        /// The SuccessResult class is a sealed implementation that:
        /// <list type="bullet">
        /// <item>Stores and provides access to the success value</item>
        /// <item>Implements pattern matching for the success case</item>
        /// <item>Provides value retrieval behavior specific to successful results</item>
        /// </list>
        /// This class cannot be inherited or instantiated directly - instances should be created through
        /// the factory methods in <see cref="Result{TValue}"/>.
        /// </remarks>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public sealed class SuccessResult : Result<TValue>
        {
            /// <summary>
            /// Gets the value contained in this successful result.
            /// </summary>
            /// <value>
            /// The value that was successfully produced by the operation.
            /// </value>
            public TValue Value { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Result{TValue}.SuccessResult"/> class with the specified value.
            /// </summary>
            /// <param name="value">The value to store in this successful result.</param>
            /// <remarks>
            /// This constructor is internal to ensure Result instances are only created through the
            /// factory methods provided by <see cref="Result{TValue}"/>.
            /// </remarks>
            internal SuccessResult(TValue value)
            {
                Value = value;
            }

            /// <summary>
            /// Internal implementation of pattern matching logic for the specific result type.
            /// </summary>
            /// <typeparam name="TResult">The type of value returned by the pattern matching functions.</typeparam>
            /// <param name="success">The function to execute if the result represents success.</param>
            /// <param name="failure">The function to execute if the result represents failure.</param>
            /// <returns>The value returned by either the success or failure function.</returns>
            /// <exception cref="ArgumentNullException">Thrown when success is null.</exception>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and
            /// <see cref="Result{TValue}.FailureResult"/> to provide their specific pattern matching behavior.
            /// The public <see cref="Result{TValue}.Match{TResult}"/> method delegates to this implementation.
            /// </remarks>
            protected override TResult MatchInternal<TResult>(Func<TValue, TResult> success,
                Func<ResultError, TResult> failure)
            {
                if (success is null)
                {
                    throw new ArgumentNullException(nameof(success));
                }

                return success(Value);
            }

            /// <summary>
            /// Internal implementation of value retrieval logic for the specific result type.
            /// </summary>
            /// <param name="value">When this method returns, contains the value if the result represents success,
            /// or the default value of <c>TValue</c> if the result represents failure.</param>
            /// <returns>true if the result represents success and the value was retrieved; otherwise, false.</returns>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and
            /// <see cref="Result{TValue}.FailureResult"/> to provide their specific value retrieval behavior.
            /// The public <see cref="Result{TValue}.TryGetValue"/> method delegates to this implementation.
            /// </remarks>
            protected override bool TryGetValueInternal(out TValue value)
            {
                value = Value;
                return true;
            }

            /// <summary>
            /// Internal implementation of error retrieval logic for the specific result type.
            /// </summary>
            /// <param name="error">When this method returns, contains the error if the result represents failure,
            /// or the default <see cref="ResultError"/> value if the result represents success.</param>
            /// <returns>true if the result represents failure and the error was retrieved; otherwise, false.</returns>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and
            /// <see cref="Result{TValue}.FailureResult"/> to provide their specific error retrieval behavior.
            /// The public <see cref="Result{TValue}.TryGetError"/> method delegates to this implementation.
            /// </remarks>
            protected override bool TryGetErrorInternal(out ResultError error)
            {
                error = default;
                return false;
            }
        }

        /// <summary>
        /// Represents a failed result containing an error description.
        /// This class provides a concrete implementation for failed operations in the Result pattern.
        /// </summary>
        /// <remarks>
        /// The FailureResult class is a sealed implementation that:
        /// <list type="bullet">
        /// <item>Stores and provides access to the error information</item>
        /// <item>Implements pattern matching for the failure case</item>
        /// <item>Provides error retrieval behavior specific to failed results</item>
        /// </list>
        /// This class cannot be inherited or instantiated directly - instances should be created through
        /// the factory methods in <see cref="Result{TValue}"/>.
        /// </remarks>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public sealed class FailureResult : Result<TValue>
        {
            /// <summary>
            /// Gets the error information contained in this failed result.
            /// </summary>
            /// <value>
            /// The error that describes why the operation failed.
            /// </value>
            public ResultError Error { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Result{TValue}.FailureResult"/> class with the specified error.
            /// </summary>
            /// <param name="error">The error information describing why the operation failed.</param>
            /// <remarks>
            /// This constructor is internal to ensure Result instances are only created through the
            /// factory methods provided by <see cref="Result{TValue}"/>.
            /// </remarks>
            internal FailureResult(ResultError error)
            {
                Error = error;
            }

            // Override abstract methods
            /// <summary>
            /// Internal implementation of pattern matching logic for the specific result type.
            /// </summary>
            /// <typeparam name="TResult">The type of value returned by the pattern matching functions.</typeparam>
            /// <param name="success">The function to execute if the result represents success.</param>
            /// <param name="failure">The function to execute if the result represents failure.</param>
            /// <returns>The value returned by either the success or failure function.</returns>
            /// <exception cref="ArgumentNullException">Thrown when failure is null.</exception>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and <see cref="Result{TValue}.FailureResult"/>
            /// to provide their specific pattern matching behavior. The public <see cref="Result{TValue}.Match{TResult}"/> method
            /// delegates to this implementation.
            /// </remarks>
            protected override TResult MatchInternal<TResult>(Func<TValue, TResult> success,
                Func<ResultError, TResult> failure)
            {
                if (failure is null)
                {
                    throw new ArgumentNullException(nameof(failure));
                }

                return failure(Error);
            }

            /// <summary>
            /// Internal implementation of value retrieval logic for the specific result type.
            /// </summary>
            /// <param name="value">When this method returns, contains the value if the result represents success,
            /// or the default value of <c>TValue</c> if the result represents failure.</param>
            /// <returns>true if the result represents success and the value was retrieved; otherwise, false.</returns>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and
            /// <see cref="Result{TValue}.FailureResult"/> to provide their specific value retrieval behavior.
            /// The public <see cref="Result{TValue}.TryGetValue"/> method delegates to this implementation.
            /// </remarks>
            protected override bool TryGetValueInternal(out TValue value)
            {
                value = default!;
                return false;
            }

            /// <summary>
            /// Internal implementation of error retrieval logic for the specific result type.
            /// </summary>
            /// <param name="error">When this method returns, contains the error if the result represents failure,
            /// or the default <see cref="ResultError"/> value if the result represents success.</param>
            /// <returns>true if the result represents failure and the error was retrieved; otherwise, false.</returns>
            /// <remarks>
            /// This protected abstract method is implemented by <see cref="Result{TValue}.SuccessResult"/> and
            /// <see cref="Result{TValue}.FailureResult"/> to provide their specific error retrieval behavior.
            /// The public <see cref="Result{TValue}.TryGetError"/> method delegates to this implementation.
            /// </remarks>
            protected override bool TryGetErrorInternal(out ResultError error)
            {
                error = Error;
                return true;
            }
        }
    }
}