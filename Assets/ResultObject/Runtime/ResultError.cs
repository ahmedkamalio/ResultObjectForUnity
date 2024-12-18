#nullable enable

using System;

namespace ResultObject
{
    /// <summary>
    /// Represents an error result in the Result pattern, containing a message, an optional error code,
    /// and an optional internal exception.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a lightweight struct designed for performance:
    /// - Zero heap allocations
    /// - Efficient equality comparisons
    /// - Minimal memory footprint
    /// Thread-safety: This struct is immutable and thread-safe.
    /// </para>
    /// 
    /// <para>
    /// The ResultError struct provides a standardized way to represent errors in the Result pattern.
    /// It can be implicitly converted to and from strings for convenience, while maintaining
    /// type safety and proper error handling semantics. The struct can also store the original
    /// exception that caused the error, preserving the full error context.
    /// </para>
    /// </remarks>
    /// <example>
    /// Basic usage:
    /// <code>
    /// // Create from message and code
    /// var error = new ResultError("File not found", "IO_ERROR");
    /// Console.WriteLine(error); // Outputs: [IO_ERROR] File not found
    /// 
    /// // Create from exception
    /// try 
    /// {
    ///     throw new FileNotFoundException("Config file missing");
    /// }
    /// catch (Exception ex)
    /// {
    ///     var errorFromException = new ResultError(ex);
    /// }
    /// 
    /// // Implicit string conversion
    /// ResultError fromString = "Operation failed";
    /// string errorMessage = error;
    /// </code>
    /// </example>
    [Serializable]
    public readonly struct ResultError : IEquatable<ResultError>
    {
        /// <summary>
        /// Gets the error message describing what went wrong.
        /// </summary>
        /// <value>A non-null string containing the error message.</value>
        public string Message { get; }

        /// <summary>
        /// Gets the optional error code that can be used for error categorization or handling.
        /// </summary>
        /// <value>A string containing the error code, or null if no code was specified.</value>
        public string? Code { get; }

        /// <summary>
        /// Gets the optional internal exception that caused this error.
        /// </summary>
        /// <value>The original exception that caused this error, or null if no exception was provided.</value>
        public Exception? InternalException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> struct.
        /// </summary>
        /// <param name="message">The error message describing what went wrong.</param>
        /// <param name="code">An optional error code for categorization.</param>
        /// <param name="exception">An optional exception that caused this error.</param>
        public ResultError(string message, string? code = null, Exception? exception = null)
        {
            Message = message;
            Code = code;
            InternalException = exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultError"/> struct from an exception.
        /// </summary>
        /// <param name="exception">The exception to create the error from. The exception's message will be used as the error message.</param>
        /// <remarks>
        /// This constructor creates a ResultError using the exception's message and stores the original exception
        /// for context preservation. The error code will be null.
        /// </remarks>
        public ResultError(Exception exception)
        {
            Message = exception.Message;
            Code = null;
            InternalException = exception;
        }

        /// <summary>
        /// Implicitly converts a <see cref="ResultError"/> to its string representation.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A string representation of the error, including the code if present.</returns>
        /// <remarks>
        /// The string representation includes only the message and code (if present).
        /// The internal exception is not included in the string representation.
        /// </remarks>
        public static implicit operator string(ResultError error) => error.ToString();

        /// <summary>
        /// Implicitly converts a string to a <see cref="ResultError"/> with no error code or exception.
        /// </summary>
        /// <param name="error">The error message to convert.</param>
        /// <returns>A new ResultError instance with the specified message, no error code, and no internal exception.</returns>
        public static implicit operator ResultError(string error) => new(error);

        /// <summary>
        /// Determines whether two specified ResultError instances are equal.
        /// </summary>
        /// <param name="left">The first ResultError to compare.</param>
        /// <param name="right">The second ResultError to compare.</param>
        /// <returns>true if the specified ResultError instances are equal; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// Equality comparison follows value semantics:
        /// - Message comparison is case-sensitive
        /// - Code comparison is case-sensitive
        /// - Exception comparison only considers Type and Message
        /// - Stack traces and other exception details are not compared
        /// </para>
        /// 
        /// <para>
        /// Special cases:
        /// - Two null Codes are considered equal
        /// - Two null InternalExceptions are considered equal
        /// - A null InternalException is not equal to any non-null InternalException
        /// </para>
        ///
        /// <para>
        /// Equality comparison includes:
        /// <list type="bullet">
        /// <item>Message property equality</item>
        /// <item>Code property equality</item>
        /// <item>InternalException type equality (if present)</item>
        /// <item>InternalException message equality (if present)</item>
        /// </list>
        /// For exceptions, only the type and message are compared, not the full exception details.
        /// </para>
        /// </remarks>
        public static bool operator ==(ResultError left, ResultError right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified ResultError instances are not equal.
        /// </summary>
        /// <param name="left">The first ResultError to compare.</param>
        /// <param name="right">The second ResultError to compare.</param>
        /// <returns>true if the specified ResultError instances are not equal; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// Equality comparison follows value semantics:
        /// - Message comparison is case-sensitive
        /// - Code comparison is case-sensitive
        /// - Exception comparison only considers Type and Message
        /// - Stack traces and other exception details are not compared
        /// </para>
        /// 
        /// <para>
        /// Special cases:
        /// - Two null Codes are considered equal
        /// - Two null InternalExceptions are considered equal
        /// - A null InternalException is not equal to any non-null InternalException
        /// </para>
        ///
        /// <para>
        /// Equality comparison includes:
        /// <list type="bullet">
        /// <item>Message property equality</item>
        /// <item>Code property equality</item>
        /// <item>InternalException type equality (if present)</item>
        /// <item>InternalException message equality (if present)</item>
        /// </list>
        /// For exceptions, only the type and message are compared, not the full exception details.
        /// </para>
        /// </remarks>
        public static bool operator !=(ResultError left, ResultError right) => !left.Equals(right);

        /// <summary>
        /// Determines whether the specified ResultError is equal to the current ResultError.
        /// </summary>
        /// <param name="other">The ResultError to compare with the current ResultError.</param>
        /// <returns>true if the specified ResultError is equal to the current ResultError; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// Equality comparison follows value semantics:
        /// - Message comparison is case-sensitive
        /// - Code comparison is case-sensitive
        /// - Exception comparison only considers Type and Message
        /// - Stack traces and other exception details are not compared
        /// </para>
        /// 
        /// <para>
        /// Special cases:
        /// - Two null Codes are considered equal
        /// - Two null InternalExceptions are considered equal
        /// - A null InternalException is not equal to any non-null InternalException
        /// </para>
        /// 
        /// <para>
        /// The equality comparison considers:
        /// <list type="bullet">
        /// <item>The Message property must match exactly</item>
        /// <item>The Code property must match exactly (or both be null)</item>
        /// <item>If either ResultError has an InternalException:
        ///   <list type="bullet">
        ///     <item>The exception types must match</item>
        ///     <item>The exception messages must match</item>
        ///   </list>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public bool Equals(ResultError other) =>
            Message == other.Message && Code == other.Code &&
            InternalException?.GetType() == other.InternalException?.GetType() &&
            InternalException?.Message == other.InternalException?.Message;

        /// <summary>
        /// Determines whether the specified object is equal to the current ResultError.
        /// </summary>
        /// <param name="obj">The object to compare with the current ResultError.</param>
        /// <returns>true if the specified object is a ResultError and equals the current ResultError; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// Equality comparison follows value semantics:
        /// - Message comparison is case-sensitive
        /// - Code comparison is case-sensitive
        /// - Exception comparison only considers Type and Message
        /// - Stack traces and other exception details are not compared
        /// </para>
        /// 
        /// <para>
        /// Special cases:
        /// - Two null Codes are considered equal
        /// - Two null InternalExceptions are considered equal
        /// - A null InternalException is not equal to any non-null InternalException
        /// </para>
        /// 
        /// <para>
        /// The equality comparison considers:
        /// <list type="bullet">
        /// <item>The Message property must match exactly</item>
        /// <item>The Code property must match exactly (or both be null)</item>
        /// <item>If either ResultError has an InternalException:
        ///   <list type="bullet">
        ///     <item>The exception types must match</item>
        ///     <item>The exception messages must match</item>
        ///   </list>
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public override bool Equals(object? obj) => obj is ResultError other && Equals(other);

        /// <summary>
        /// Returns a hash code for the current ResultError.
        /// </summary>
        /// <returns>A hash code for the current ResultError.</returns>
        /// <remarks>
        /// The hash code is computed using:
        /// <list type="bullet">
        /// <item>Message property</item>
        /// <item>Code property</item>
        /// <item>InternalException type (if present)</item>
        /// <item>InternalException message (if present)</item>
        /// </list>
        /// This ensures the hash code computation is consistent with equality comparison.
        /// </remarks>
        public override int GetHashCode() => HashCode.Combine(
            Message,
            Code,
            InternalException?.GetType(),
            InternalException?.Message);

        /// <summary>
        /// Returns a string representation of the current ResultError.
        /// </summary>
        /// <returns>
        /// If Code is null, returns just the Message.
        /// If Code is not null, returns <c>"[Code] Message"</c>.
        /// </returns>
        /// <remarks>
        /// The string representation includes only the message and code (if present).
        /// The internal exception is not included in the string representation.
        /// </remarks>
        /// <example>
        /// <code>
        /// var error1 = new ResultError("Not found", "404");
        /// Console.WriteLine(error1); // Outputs: [404] Not found
        /// 
        /// var error2 = new ResultError("Failed", null);
        /// Console.WriteLine(error2); // Outputs: Failed
        /// 
        /// var error3 = new ResultError(new FileNotFoundException("File missing"));
        /// Console.WriteLine(error3); // Outputs: File missing
        /// </code>
        /// </example>
        public override string ToString() => Code is null ? Message : $"[{Code}] {Message}";
    }
}