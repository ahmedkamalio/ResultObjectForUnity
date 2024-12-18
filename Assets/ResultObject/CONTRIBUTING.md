# Contributing to ResultObject

Thank you for your interest in contributing to ResultObject! This document provides guidelines and information about
contributing to this Unity package.

## Code of Conduct

By participating in this project, you are expected to uphold our Code of Conduct:

- Be respectful and inclusive
- Exercise empathy and kindness
- Accept constructive criticism
- Focus on what is best for the community
- Show courtesy and respect towards other community members

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue on GitHub with the following information:

1. Clear and descriptive title
2. Detailed description of the bug
3. Steps to reproduce
4. Expected behavior
5. Actual behavior
6. Unity version and any relevant environment details
7. Code samples or test cases demonstrating the issue

### Suggesting Enhancements

We welcome suggestions for enhancements! When creating an enhancement suggestion, please include:

1. Clear and descriptive title
2. Detailed explanation of the proposed functionality
3. Any potential implementation approaches you've considered
4. Example use cases
5. Why this enhancement would be useful to most users

### Pull Requests

#### Getting Started

1. Fork the repository
2. Create a new branch from `main` for your changes
3. Make your changes following our coding standards
4. Write or update tests as needed
5. Update documentation as needed
6. Submit a pull request

#### Pull Request Requirements

All pull requests must:

1. Target the `main` branch
2. Include adequate tests
3. Update relevant documentation
4. Follow the existing code style
5. Include XML documentation for public APIs
6. Pass all existing tests
7. Not decrease code coverage
8. Include relevant updates to the changelog

### Coding Standards

#### General Guidelines

- Follow C# coding conventions
- Use meaningful names for variables, methods, and classes
- Keep methods focused and concise
- Write self-documenting code
- Add comments only when necessary to explain complex logic
- Use XML documentation for all public APIs

#### Specific Requirements

1. **Immutability**
    - All new types should be immutable
    - Use readonly fields and properties
    - Return new instances instead of modifying existing ones

2. **Thread Safety**
    - All public APIs must be thread-safe
    - Document any thread-safety considerations

3. **Performance**
    - Minimize allocations
    - Use value types appropriately
    - Consider using struct for small, simple types
    - Profile performance-critical code

4. **Error Handling**
    - Use the Result pattern consistently
    - Avoid throwing exceptions except for truly exceptional cases
    - Document all possible error conditions

5. **Testing**
    - Write unit tests for all new functionality
    - Follow the existing test structure
    - Include both positive and negative test cases
    - Test edge cases and error conditions

#### XML Documentation

All public APIs must include XML documentation with:

```csharp
/// <summary>
/// Clear description of purpose
/// </summary>
/// <remarks>
/// Additional information about usage, implementation, etc.
/// </remarks>
/// <param name="paramName">Parameter description</param>
/// <returns>Description of return value</returns>
/// <exception cref="ExceptionType">When exception can occur</exception>
/// <example>
/// Usage example in code
/// </example>
```

### Testing Guidelines

1. Place tests in the `ResultObject.Tests` assembly
2. Follow the existing test structure and naming conventions
3. Use descriptive test names that indicate:
    - The scenario being tested
    - The expected behavior
    - Any important conditions
4. Use the Arrange-Act-Assert pattern
5. Keep tests focused and independent
6. Use meaningful test data

### Documentation

1. Update README.md for significant changes
2. Update API documentation for new features
3. Add migration guides for breaking changes
4. Update examples to reflect changes
5. Keep documentation clear and concise

### Commit Messages

- Use clear and meaningful commit messages
- Follow conventional commits format:
    - `feat: add new feature`
    - `fix: resolve specific issue`
    - `docs: update documentation`
    - `test: add or modify tests`
    - `refactor: improve code structure`
    - `perf: improve performance`
    - `style: format code`

### Development Workflow

1. **Fork and Clone**
   ```bash
   git clone https://github.com/yourusername/ResultObjectForUnity.git
   cd ResultObjectForUnity
   ```

2. **Create a Branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Changes**
    - Write code
    - Add tests
    - Update documentation

4. **Test**
    - Run all tests
    - Ensure no existing tests are broken
    - Verify code coverage

5. **Submit**
    - Push changes to your fork
    - Create a pull request
    - Respond to review feedback

### License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Questions?

If you have questions about contributing, please create a discussion in the GitHub repository.

Thank you for helping improve ResultObject!
