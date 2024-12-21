#nullable enable

using System;
using NUnit.Framework;

namespace ResultObject.Tests
{
    [TestFixture]
    public class ResultTests
    {
        #region Constructor Tests

        [Test]
        public void Success_WithValue_CreatesSuccessResult()
        {
            // Arrange
            const int testValue = 42;

            // Act
            var result = Result.Success(testValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.True(result.TryGetValue(out var value));
            Assert.AreEqual(value, testValue);
        }

        [Test]
        public void Failure_WithMessage_CreatesFailureResult()
        {
            // Arrange
            const string errorMessage = "Test error";

            // Act
            var result = Result.Failure<int>(errorMessage);

            // Assert
            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.True(result.TryGetError(out var error));
            Assert.AreEqual(error.Message, errorMessage);
        }

        [Test]
        public void Failure_WithMessageAndCode_CreatesFailureResult()
        {
            // Arrange
            const string errorMessage = "Test error";
            const string errorCode = "TEST_001";

            // Act
            var result = Result.Failure<string>(errorMessage, errorCode);

            // Assert
            Assert.True(result.TryGetError(out var error));
            Assert.AreEqual(error.Message, errorMessage);
            Assert.AreEqual(error.Code, errorCode);
        }

        [Test]
        public void Failure_WithException_CreatesFailureResult()
        {
            // Arrange
            var exception = new InvalidOperationException("Test exception");

            // Act
            var result = Result.Failure<int>(exception);

            // Assert
            Assert.True(result.TryGetError(out var error));
            Assert.AreEqual(error.Message, exception.Message);
            Assert.AreSame(error.InternalException, exception);
        }

        #endregion

        #region Try Method Tests

        [Test]
        public void Try_SuccessfulOperation_ReturnsSuccessResult()
        {
            // Arrange
            const int expectedValue = 42;

            // Act
            var result = Result.Try(() => expectedValue);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.TryGetValue(out var value));
            Assert.AreEqual(value, expectedValue);
        }

        [Test]
        public void Try_FailedOperation_ReturnsFailureResult()
        {
            // Arrange
            var expectedException = new InvalidOperationException("Test exception");

            // Act
            var result = Result.Try<int>(() => throw expectedException);

            // Assert
            Assert.True(result.IsFailure);
            Assert.True(result.TryGetError(out var error));
            Assert.AreSame(error.InternalException, expectedException);
        }

        #endregion

        #region Map and Bind Tests

        [Test]
        public void Map_SuccessResult_TransformsValue()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var mapped = result.Map(x => x.ToString());

            // Assert
            Assert.True(mapped.IsSuccess);
            Assert.True(mapped.TryGetValue(out var value));
            Assert.AreEqual(value, "42");
        }

        [Test]
        public void Map_FailureResult_PropagatesError()
        {
            // Arrange
            var actualError = new ResultError("Test error");
            var result = Result.Failure<int>(actualError);

            // Act
            var mapped = result.Map(x => x.ToString());

            // Assert
            Assert.True(result.TryGetError(out var error));
            Assert.True(mapped.IsFailure);
            Assert.AreEqual(error, actualError);
        }

        [Test]
        public void Bind_SuccessResult_ChainsOperations()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var bound = result.Bind(x => Result.Success(x.ToString()));

            // Assert
            Assert.True(bound.IsSuccess);
            Assert.True(bound.TryGetValue(out var value));
            Assert.AreEqual(value, "42");
        }

        [Test]
        public void Bind_FailureResult_PropagatesError()
        {
            // Arrange
            var actualError = new ResultError("Test error");
            var result = Result.Failure<int>(actualError);

            // Act
            var bound = result.Bind(x => Result.Success(x.ToString()));

            // Assert
            Assert.True(result.TryGetError(out var error));
            Assert.True(bound.IsFailure);
            Assert.AreEqual(error, actualError);
        }

        #endregion

        #region Pattern Matching Tests

        [Test]
        public void Match_SuccessResult_ExecutesSuccessFunction()
        {
            // Arrange
            var result = Result.Success(42);
            const string expected = "Success: 42";

            // Act
            var matched = result.Match(
                success: value => $"Success: {value}",
                failure: error => $"Error: {error}");

            // Assert
            Assert.AreEqual(matched, expected);
        }

        [Test]
        public void Match_FailureResult_ExecutesFailureFunction()
        {
            // Arrange
            var error = new ResultError("Test error");
            var result = Result.Failure<int>(error);
            var expected = $"Error: {error}";

            // Act
            var matched = result.Match(
                success: value => $"Success: {value}",
                failure: err => $"Error: {err}");

            // Assert
            Assert.AreEqual(matched, expected);
        }

        #endregion

        #region Callback Tests

        [Test]
        public void OnSuccess_SuccessResult_ExecutesCallback()
        {
            // Arrange
            var result = Result.Success(42);
            var callbackExecuted = false;

            // Act
            result.OnSuccess(_ => callbackExecuted = true);

            // Assert
            Assert.True(callbackExecuted);
        }

        [Test]
        public void OnSuccess_FailureResult_DoesNotExecuteCallback()
        {
            // Arrange
            var result = Result.Failure<int>("Test error");
            var callbackExecuted = false;

            // Act
            result.OnSuccess(_ => callbackExecuted = true);

            // Assert
            Assert.False(callbackExecuted);
        }

        [Test]
        public void OnFailure_FailureResult_ExecutesCallback()
        {
            // Arrange
            var result = Result.Failure<int>("Test error");
            var callbackExecuted = false;

            // Act
            result.OnFailure(_ => callbackExecuted = true);

            // Assert
            Assert.True(callbackExecuted);
        }

        [Test]
        public void OnFailure_SuccessResult_DoesNotExecuteCallback()
        {
            // Arrange
            var result = Result.Success(42);
            var callbackExecuted = false;

            // Act
            result.OnFailure(_ => callbackExecuted = true);

            // Assert
            Assert.False(callbackExecuted);
        }

        #endregion

        #region ResultError Tests

        [Test]
        public void ResultError_ImplicitStringConversion_WorksCorrectly()
        {
            // Arrange
            const string message = "Test error";
            var error = new ResultError(message);

            // Act
            string errorString = error;

            // Assert
            Assert.AreEqual(errorString, message);
        }

        [Test]
        public void ResultError_WithCode_FormatsToStringCorrectly()
        {
            // Arrange
            const string message = "Test error";
            const string code = "TEST_001";
            var error = new ResultError(message, code);

            // Act
            var errorString = error.ToString();

            // Assert
            Assert.AreEqual(errorString, $"[{code}] {message}");
        }

        [Test]
        public void ResultError_Equality_WorksCorrectly()
        {
            // Arrange
            var error1 = new ResultError("Test error", "TEST_001");
            var error2 = new ResultError("Test error", "TEST_001");
            var error3 = new ResultError("Different error", "TEST_001");

            // Assert
            Assert.AreEqual(error1, error2);
            Assert.AreNotEqual(error1, error3);
        }

        #endregion

        #region Unit Tests

        [Test]
        public void Unit_AllInstancesAreEqual()
        {
            // Arrange
            var unit1 = Unit.Value;
            var unit2 = default(Unit);

            // Assert
            Assert.AreEqual(unit1, unit2);
            Assert.True(unit1 == unit2);
            Assert.False(unit1 != unit2);
        }

        [Test]
        public void UnitResult_Success_CreatesSuccessResult()
        {
            // Act
            var result = Result.Unit();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.TryGetValue(out var value));
            Assert.AreEqual(value, Unit.Value);
        }

        [Test]
        public void UnitResult_Failure_CreatesFailureResult()
        {
            // Arrange
            var actualError = new ResultError("Test error");

            // Act
            var result = Result.Unit(actualError);

            // Assert
            Assert.True(result.IsFailure);
            Assert.True(result.TryGetError(out var error));
            Assert.AreEqual(error, actualError);
        }

        #endregion

        #region Deconstruction Tests

        [Test]
        public void Deconstruct_SuccessResult_SetsCorrectValues()
        {
            // Arrange
            var result = Result.Success(42);

            // Act
            var (isSuccess, value, _) = result;

            // Assert
            Assert.True(isSuccess);
            Assert.AreEqual(value, 42);
        }

        [Test]
        public void Deconstruct_FailureResult_SetsCorrectValues()
        {
            // Arrange
            var expectedError = new ResultError("Test error");
            var result = Result.Failure<int>(expectedError);

            // Act
            var (isSuccess, value, error) = result;

            // Assert
            Assert.False(isSuccess);
            Assert.AreEqual(value, 0);
            Assert.AreEqual(error, expectedError);
        }

        #endregion

        #region ResultError Operator and Method Tests

        [Test]
        public void ResultError_ImplicitOperator_CreatesErrorFromString()
        {
            // Arrange
            const string errorMessage = "Test error message";

            // Act
            ResultError error = errorMessage;

            // Assert
            Assert.AreEqual(errorMessage, error.Message);
            Assert.IsNull(error.Code);
            Assert.IsNull(error.InternalException);
        }

        [Test]
        public void ResultError_EqualityOperator_ComparesCorrectly()
        {
            // Arrange
            var error1 = new ResultError("Error", "CODE1");
            var error2 = new ResultError("Error", "CODE1");
            var error3 = new ResultError("Different", "CODE1");

            // Assert
            Assert.True(error1 == error2);
            Assert.False(error1 == error3);
        }

        [Test]
        public void ResultError_InequalityOperator_ComparesCorrectly()
        {
            // Arrange
            var error1 = new ResultError("Error", "CODE1");
            var error2 = new ResultError("Error", "CODE1");
            var error3 = new ResultError("Different", "CODE1");

            // Assert
            Assert.False(error1 != error2);
            Assert.True(error1 != error3);
        }

        [Test]
        public void ResultError_EqualsObject_HandlesNullAndTypeMismatch()
        {
            // Arrange
            var error = new ResultError("Error");

            // Assert
            Assert.False(error.Equals(null!));
            Assert.False(error.Equals("not a ResultError"));
            Assert.True(error.Equals((object)new ResultError("Error")));
        }

        [Test]
        public void ResultError_GetHashCode_ReturnsConsistentValue()
        {
            // Arrange
            var error1 = new ResultError("Error", "CODE1");
            var error2 = new ResultError("Error", "CODE1");
            var error3 = new ResultError("Different", "CODE1");

            // Assert
            Assert.AreEqual(error1.GetHashCode(), error2.GetHashCode());
            Assert.AreNotEqual(error1.GetHashCode(), error3.GetHashCode());
        }

        #endregion

        #region Exception Tests for Null Arguments

        [Test]
        public void Try_NullAction_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Result.Try<int>(null!));
        }

        [Test]
        public void Map_NullMapper_ThrowsArgumentNullException()
        {
            // Arrange
            var result = Result.Success(42);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.Map<string>(null!));
        }

        [Test]
        public void Bind_NullBinder_ThrowsArgumentNullException()
        {
            // Arrange
            var result = Result.Success(42);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.Bind<string>(null!));
        }

        [Test]
        public void MatchInternal_NullSuccessFunction_ThrowsArgumentNullException()
        {
            // Arrange
            var result = Result.Success(42);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.Match<string>(
                success: null!,
                failure: _ => "failure"
            ));
        }

        [Test]
        public void MatchInternal_NullFailureFunction_ThrowsArgumentNullException()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.Match<string>(
                success: _ => "success",
                failure: null!
            ));
        }

        #endregion

        #region Unit Value Type Tests

        [Test]
        public void Unit_EqualsObject_HandlesNullAndTypeMismatch()
        {
            // Arrange
            var unit = Unit.Value;

            // Assert
            Assert.False(unit.Equals(null));
            Assert.True(unit.Equals((object)default(Unit)));
        }

        [Test]
        public void Unit_GetHashCode_ReturnsZero()
        {
            // Arrange
            var unit1 = Unit.Value;
            var unit2 = default(Unit);

            // Assert
            Assert.AreEqual(0, unit1.GetHashCode());
            Assert.AreEqual(0, unit2.GetHashCode());
            Assert.AreEqual(unit1.GetHashCode(), unit2.GetHashCode());
        }

        #endregion
    }
}