# ResultObject

ResultObject is a .NET package that provides a robust implementation of the Result pattern for Unity projects.
It offers a type-safe way to handle operation outcomes without throwing exceptions, promoting clean error handling and
maintainable code.

## Features

- Type-safe error handling without exceptions
- Explicit handling of both success and failure cases
- Composable operations through functional programming patterns
- Clean and maintainable error propagation
- Thread-safe and immutable implementations

## Installation

Add this package to your Unity project through the Package Manager.

## Basic Usage

### Creating Results

```csharp
// Success cases
Result<int> success = Result.Success(42);
Result<Unit> unitSuccess = Result.Unit(); // For void operations

// Failure cases
Result<int> failure = Result.Failure<int>("Operation failed");
Result<int> codedFailure = Result.Failure<int>("Not found", "404");
Result<Unit> unitFailure = Result.Unit(new ResultError("Operation failed"));
```

### Handling Results

```csharp
// Pattern matching
string message = result.Match(
    success: value => $"Success: {value}",
    failure: error => $"Error: {error}"
);

// Using TryGet methods
if (result.TryGetValue(out var value))
{
    Console.WriteLine($"Got value: {value}");
}

if (result.TryGetError(out var error))
{
    Console.WriteLine($"Got error: {error}");
}

// Using deconstruction
var (isSuccess, value, error) = result;
```

### Chaining Operations

```csharp
var finalResult = GetUser()
    .Bind(UpdateUser)
    .Bind(SaveUser);

// Transform values
var stringResult = integerResult.Map(x => x.ToString());

// Handle success/failure
result
    .OnSuccess(value => Console.WriteLine($"Success: {value}"))
    .OnFailure(error => Console.WriteLine($"Error: {error}"));
```

### Exception Handling

```csharp
// Safely execute code that might throw
Result<int> result = Result.Try(() => 
{
    return int.Parse("123");
});

// Create failure from exception
try 
{
    // Some operation
}
catch (Exception ex)
{
    return Result.Failure<MyType>(ex);
}
```

## Core Types

### Result<TValue>

The main class that implements the Result pattern. It's a discriminated union that contains either a success value of
type `TValue` or an error.

```csharp
public interface IResult<TValue>
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    bool TryGetValue(out TValue value);
    bool TryGetError(out ResultError error);
    Result<TNew> Map<TNew>(Func<TValue, TNew> mapper);
    Result<TNew> Bind<TNew>(Func<TValue, Result<TNew>> binder);
    Result<TValue> OnSuccess(Action<TValue> action);
    Result<TValue> OnFailure(Action<ResultError> action);
    TResult Match<TResult>(Func<TValue, TResult> success, Func<ResultError, TResult> failure);
    void Deconstruct(out bool isSuccess, out TValue? value, out ResultError error);
}
```

### ResultError

A lightweight struct that represents error information, containing a message, an optional error code, and an optional
internal exception.

```csharp
public readonly struct ResultError
{
    public string Message { get; }
    public string? Code { get; }
    public Exception? InternalException { get; }
}
```

### Unit

A zero-byte struct that represents void in generic contexts, particularly useful for operations that don't return a
value but need to indicate success/failure.

```csharp
public readonly struct Unit
{
    public static Unit Value => default;
}
```

## Best Practices

1. **Prefer Results Over Exceptions**: Use Result<T> for expected failure cases, reserving exceptions for truly
   exceptional circumstances.

2. **Return Early**: When creating methods that return Results, check for failure conditions early and return failure
   Results immediately.

   ```csharp
   public Result<User> GetUser(string id)
   {
       if (string.IsNullOrEmpty(id))
           return Result.Failure<User>("ID cannot be empty");
   
       // Continue with operation...
   }
   ```

3. **Use Meaningful Error Codes**: When creating failure Results, include error codes that can be used for error
   handling and categorization.

   ```csharp
   return Result.Failure<User>("User not found", "USER_404");
   ```

4. **Chain Operations**: Take advantage of the functional programming features to create clean, readable chains of
   operations.

   ```csharp
   return GetUser(id)
       .Map(user => user.WithUpdatedName(newName))
       .Bind(user => SaveUser(user));
   ```

5. **Use Unit for Void Operations**: When an operation doesn't need to return a value but could fail, use Result<Unit>.

   ```csharp
   public Result<Unit> DeleteUser(string id)
   {
       if (!userExists(id))
           return Result.Unit(new ResultError("User not found", "USER_404"));

       // Delete user
       return Result.Unit();
   }
   ```

## Thread Safety

All types in this package are immutable and thread-safe. You can safely pass Results between threads and use them in
concurrent operations without additional synchronization.

## Performance Considerations

- The `Unit` struct is zero-byte and optimized for performance
- `ResultError` is a lightweight struct designed for minimal overhead
- `Result<T>` uses sealed classes for concrete implementations to enable devirtualization
- All operations are allocation-free where possible

## Contributing

Please refer to the project's contribution guidelines for information about contributing to this package.

## License

This validation library is licensed under the MIT License - see
the [LICENSE](https://raw.githubusercontent.com/ahmedkamalio/ResultObjectForUnity/refs/heads/main/Assets/ResultObject/LICENSE)
file for details.
