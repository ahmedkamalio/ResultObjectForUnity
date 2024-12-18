#nullable enable

using System;

namespace ResultObject
{
    /// <summary>
    /// Represents a void type that indicates no value.
    /// </summary>
    /// <remarks>
    /// Unit is a zero-byte struct optimized for performance:
    /// - All instances are bitwise identical
    /// - Default struct optimization applies
    /// - No memory allocation occurs
    /// - Ideal for use in functional programming patterns
    /// 
    /// Common use cases:
    /// - Representing void in generic contexts
    /// - Indicating successful completion without a return value
    /// - Maintaining type safety in the Result pattern
    /// </remarks>
    /// <example>
    /// Basic usage with Result pattern:
    /// <code>
    /// // Return success with no value
    /// Result&lt;Unit&gt; Operation()
    /// {
    ///     // Perform some operation
    ///     return Result.Unit();
    /// }
    /// 
    /// // Using Unit in error cases
    /// Result&lt;Unit&gt; Operation()
    /// {
    ///     if (error)
    ///         return Result.Unit(new ResultError("Operation failed"));
    ///     return Result.Unit();
    /// }
    /// </code>
    /// </example>
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <summary>
        /// Gets the singleton instance of the Unit type.
        /// </summary>
        /// <value>
        /// The default value of Unit. Since Unit is a zero-byte struct,
        /// all instances are identical.
        /// </value>
        public static Unit Value => default;

        /// <summary>
        /// Determines whether the specified Unit is equal to the current Unit.
        /// Always returns true as all Unit values are equal.
        /// </summary>
        /// <param name="other">The Unit to compare with the current Unit.</param>
        /// <returns>Always returns true, as all Unit values are equal.</returns>
        public bool Equals(Unit other) => true;

        /// <summary>
        /// Determines whether the specified object is equal to the current Unit.
        /// </summary>
        /// <param name="obj">The object to compare with the current Unit.</param>
        /// <returns>true if the specified object is Unit; otherwise, false.</returns>
        public override bool Equals(object? obj) => obj is Unit;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// Always returns 0, as all Unit values are equal and should have the same hash code.
        /// </returns>
        public override int GetHashCode() => 0;

        /// <summary>
        /// Determines whether two specified Unit instances are equal.
        /// </summary>
        /// <param name="left">The first Unit to compare.</param>
        /// <param name="right">The second Unit to compare.</param>
        /// <returns>Always returns true, as all Unit values are equal.</returns>
        public static bool operator ==(Unit left, Unit right) => true;

        /// <summary>
        /// Determines whether two specified Unit instances are not equal.
        /// </summary>
        /// <param name="left">The first Unit to compare.</param>
        /// <param name="right">The second Unit to compare.</param>
        /// <returns>Always returns false, as all Unit values are equal.</returns>
        public static bool operator !=(Unit left, Unit right) => false;
    }
}