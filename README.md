# Yash.FluentDataPipelines

[![.NET](https://img.shields.io/badge/.NET-Portable-blue.svg)](https://dotnet.microsoft.com/)
[![UiPath](https://img.shields.io/badge/UiPath-25.10+-green.svg)](https://www.uipath.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

A powerful, fluent API library for building data pipelines in UiPath with support for extraction, validation, transformation, and formatting operations. Transform your data processing workflows with a clean, chainable syntax that propagates validation state throughout the pipeline.

## Features

- 🔄 **Fluent API Design** - Chain operations together in a readable, expressive syntax
- ✅ **Validation State Tracking** - Failed validations don't short-circuit; errors are tracked and can be checked later
- 🎯 **Type-Safe Operations** - Full IntelliSense support with generic type constraints
- ⚙️ **Configurable Operations** - Base methods accept configuration objects for fine-grained control
- 🚀 **Convenience Methods** - Simple overloads with sensible defaults for common use cases
- 📦 **Comprehensive Type Support** - Works with strings, dates, numbers, collections, and more
- 🔒 **Immutable Operations** - All operations return new instances, ensuring thread safety
- 📝 **Rich Error Tracking** - Detailed error messages with operation context and timestamps

## Installation

### For UiPath Projects

1. Open your UiPath project in UiPath Studio
2. Go to **Manage Packages** → **All Packages**
3. Search for `Yash.FluentDataPipelines`
4. Click **Install**

### Manual Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/Yash.FluentDataPipelines/releases) page
2. Extract the `.nupkg` file
3. Add the package to your UiPath project's dependencies

## Quick Start

```csharp
using Yash.FluentDataPipelines.Extensions;
using System;

// Extract a date, transform it, validate it, and format the result
string result = "2024-12-10"
    .ExtractDate()
    .AddDays(1)
    .Before(DateTime.Now.AddDays(7))
    .Format((value, isValid) => isValid ? "Valid Date!" : "Invalid Date");

Console.WriteLine(result); // Output: "Valid Date!"
```

## Core Concepts

### PipelineValue<T>

The heart of the library is `PipelineValue<T>`, an immutable wrapper that tracks:
- **Value**: The actual data being processed
- **IsValid**: Whether the value has passed all validations
- **Errors**: Collection of errors that occurred during processing

```csharp
var pipelineValue = new PipelineValue<string>("Hello World", isValid: true);
Console.WriteLine(pipelineValue.Value);    // "Hello World"
Console.WriteLine(pipelineValue.IsValid); // True
```

### Failed State Propagation

Unlike traditional validation that throws exceptions, this library allows operations to continue even when validation fails. The failed state is tracked and can be checked at any point:

```csharp
var result = "invalid-date"
    .ExtractDate()           // Fails, but continues
    .AddDays(1)              // Still executes (no-op on invalid state)
    .Before(DateTime.Now)    // Still executes
    .Format((v, isValid) => isValid ? "OK" : "Failed"); // "Failed"

// Check errors
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.Message);
    }
}
```

## Documentation

For detailed documentation on all features and operations, see the [Documentation](docs/) folder:

- **[Core Concepts](docs/core-concepts.md)** - PipelineValue, PipelineError, CrossValidationResult, and validation state propagation
- **[Extraction Operations](docs/extraction.md)** - Extract typed values from strings with regex, fuzzy matching, and custom parsing
- **[Validation Operations](docs/validation.md)** - Validate dates, numbers, strings, and custom conditions
- **[Transformation Operations](docs/transformation.md)** - Transform dates, numbers, strings, and collections
- **[Formatting Operations](docs/formatting.md)** - Format values into strings with multiple options
- **[Collection Operations](docs/collections.md)** - Work with collections using LINQ-like operations
- **[Fuzzy Matching](docs/fuzzy-matching.md)** - Advanced fuzzy string matching, normalization, and cross-validation
- **[Configuration Reference](docs/configuration.md)** - Complete reference for all configuration objects

## Quick API Reference

### Extraction

```csharp
// Basic extraction
var date = "2024-12-10".ExtractDate();
var number = "42".ExtractInt();
var price = "99.99".ExtractDecimal();
var flag = "true".ExtractBool();
var guid = "550e8400-e29b-41d4-a716-446655440000".ExtractGuid();

// Advanced with configuration
var config = new ExtractConfig
{
    RegexPattern = @"Price: \$(\d+\.\d{2})",
    GroupIndex = 1
};
var price = "Price: $99.99".Extract<decimal>(config, (str, cfg) => 
    decimal.Parse(str, cfg.NumberStyles, cfg.Culture));
```

### Validation

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .After(DateTime.Now.AddDays(-7));

var result = "42"
    .ExtractInt()
    .GreaterThan(10)
    .LessThan(100)
    .Between(20, 50);

var result = "user@example.com"
    .Contains("@")
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$");
```

### Transformation

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .AddDays(7)
    .AddMonths(1);

var result = "99.99"
    .ExtractDecimal()
    .Multiply(1.1m)
    .Divide(2m)
    .Round(2);

var result = "  Hello World  "
    .Trim()
    .ToUpper()
    .Replace("world", "Universe");
```

### Formatting

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Format("yyyy-MM-dd");

var result = "99.99"
    .ExtractDecimal()
    .FormatCurrency();

var result = "42"
    .ExtractInt()
    .Format((value, isValid) => isValid ? $"Count: {value}" : "Invalid");
```

### Collections

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var result = numbers
    .Where(x => x > 2)
    .Select(x => x * 2)
    .First();
```

### Fuzzy Matching

```csharp
var result = "John Smith"
    .FuzzyMatch("Jon Smith", new FuzzyMatchingConfig 
    { 
        SimilarityThreshold = 0.8 
    });

var result = "123 Main St"
    .NormalizeAddress()
    .FuzzyMatch("123 Main Street");
```

## Examples

### Complex Data Pipeline

```csharp
// Extract price from text, validate, transform, and format
var result = "Product: Widget, Price: $99.99"
    .Extract<decimal>(
        new ExtractConfig
        {
            RegexPattern = @"Price: \$(\d+\.\d{2})",
            GroupIndex = 1
        },
        (str, cfg) => decimal.Parse(str, cfg.NumberStyles, cfg.Culture))
    .GreaterThan(50m)
    .LessThan(200m)
    .Multiply(1.15m)  // Add 15% tax
    .Round(2)
    .FormatCurrency();
```

### Error Handling

```csharp
var pipelineValue = "invalid-date"
    .ExtractDate()
    .AddDays(1)
    .Before(DateTime.Now);

if (!pipelineValue.IsValid)
{
    Console.WriteLine("Validation failed!");
    foreach (var error in pipelineValue.Errors)
    {
        Console.WriteLine($"[{error.Timestamp}] {error.Operation}: {error.Message}");
    }
}
```

For more examples and detailed usage patterns, see the [Documentation](docs/) folder.

## Best Practices

1. **Use Configuration Objects for Complex Scenarios**: When you need fine-grained control, use the base methods with configuration objects.

2. **Check Validation State**: Always check `IsValid` before using the value in critical operations.

3. **Review Errors**: Use the `Errors` collection to understand what went wrong in the pipeline.

4. **Leverage Type Safety**: Use the strongly-typed convenience methods when possible for better IntelliSense support.

5. **Keep Pipelines Readable**: Break long chains into multiple lines for better readability.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Built for the UiPath RPA platform
- Inspired by functional programming patterns and fluent interface design
- Thanks to all contributors who help improve this library

## Support

For issues, questions, or suggestions, please open an issue on the [GitHub repository](https://github.com/yourusername/Yash.FluentDataPipelines/issues).

---

Made with ❤️ for the UiPath community

