# ResultObject

A lightweight, zero-dependency Result pattern implementation for Unity projects. Handle operation outcomes cleanly
without exceptions.

## Features

- ✨ **Type-Safe Error Handling** - Handle failures without throwing exceptions.
- 🧩 **Composable** - Chain operations with functional programming patterns.
- ⚡ **High Performance** - Zero-allocation patterns and lightweight structs.
- 🧵 **Thread-Safe** - Immutable types safe for concurrent use.
- 📘 **Well-Documented** - Comprehensive XML documentation and examples.

## Installation

1. Open the Unity Package Manager
2. Click the '+' button in the top-left corner
3. Select "Add package from git URL"
4. Enter: `https://github.com/ahmedkamalio/ResultObjectForUnity.git?path=/Assets/ResultObject`

## Quick Start

```csharp
// Success cases
Result<int> success = Result.Success(42);
Result<string> failure = Result.Failure<string>("Not found", "404");

// Handle results
string message = result.Match(
    success: value => $"Got {value}",
    failure: error => $"Error: {error}"
);

// Chain operations
Result<User> result = GetUser(id)
    .Bind(user => UpdateUser(user))
    .Bind(user => SaveUser(user));

// Void operations with Unit
Result<Unit> SaveChanges() 
{
    if (error)
        return Result.Unit(new ResultError("Save failed"));
    return Result.Unit();
}

// Safe exception handling
Result<int> result = Result.Try(() => 
    int.Parse(input)
);
```

## Documentation

For complete documentation, see:

- [API Reference](https://github.com/ahmedkamalio/ResultObjectForUnity/blob/main/Assets/ResultObject/Documentation~/ResultObject.md)

## Contributing

Contributions are welcome! Please read
our [Contributing Guidelines](https://github.com/ahmedkamalio/ResultObjectForUnity/blob/main/Assets/ResultObject/CONTRIBUTING.md)
before submitting PRs.

## License

This validation library is licensed under the MIT License - see
the [LICENSE](https://github.com/ahmedkamalio/ResultObjectForUnity/blob/main/Assets/ResultObject/LICENSE)
file for details.
