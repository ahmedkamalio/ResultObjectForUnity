#nullable enable

using System;

namespace ResultObject
{
    /// <summary>
    /// Represents a discriminated union that encapsulates the result of an operation,
    /// containing either a success value of type <typeparamref name="TValue"/> or an error.
    /// This interface provides a type-safe way to handle operation outcomes without throwing exceptions.
    /// </summary>
    /// <typeparam name="TValue">The type of the value in case of success.</typeparam>
    /// <remarks>
    /// The IResult interface is an abstraction of the Result pattern, offering a robust way to handle
    /// operation outcomes by explicitly representing both success and failure cases. It promotes:
    /// <list type="bullet">
    /// <item>Type-safe error handling without exceptions</item>
    /// <item>Explicit handling of both success and failure cases</item>
    /// <item>Composable operations through functional programming patterns</item>
    /// <item>Clean and maintainable error propagation</item>
    /// </list>
    /// </remarks>
    public interface IResult<TValue>
    {
        /// <summary>
        /// Gets a value indicating whether the result represents a successful operation.
        /// </summary>
        /// <value>true if the result represents success; otherwise, false.</value>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets a value indicating whether the result represents a failed operation.
        /// </summary>
        /// <value>true if the result represents failure; otherwise, false.</value>
        bool IsFailure { get; }

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
        bool TryGetValue(out TValue value);

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
        bool TryGetError(out ResultError error);

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
        Result<TNew> Map<TNew>(Func<TValue, TNew> mapper);

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
        Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder);

        /// <summary>
        /// Executes the provided action if the result represents success.
        /// </summary>
        /// <param name="action">The action to execute with the success value.</param>
        /// <returns>The same result instance to enable method chaining.</returns>
        Result<TValue> OnSuccess(Action<TValue> action);

        /// <summary>
        /// Executes the provided action if the result represents failure.
        /// </summary>
        /// <param name="action">The action to execute with the error value.</param>
        /// <returns>The same result instance to enable method chaining.</returns>
        Result<TValue> OnFailure(Action<ResultError> action);

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
        TResult Match<TResult>(Func<TValue, TResult> success, Func<ResultError, TResult> failure);

        /// <summary>
        /// Deconstructs the result into its components for use with C# deconstruction syntax.
        /// </summary>
        /// <param name="isSuccess">When this method returns, indicates whether the result represents success.</param>
        /// <param name="value">When this method returns, contains the success value or default.</param>
        /// <param name="error">When this method returns, contains the error value or default.</param>
        void Deconstruct(out bool isSuccess, out TValue? value, out ResultError error);
    }
}