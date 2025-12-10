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

## API Documentation

### Extraction Operations

Extract typed values from strings with optional regex patterns and custom parsing.

#### Basic Extraction

```csharp
// Extract dates
var date = "2024-12-10".ExtractDate();

// Extract numbers
var number = "42".ExtractInt();
var price = "99.99".ExtractDecimal();

// Extract booleans
var flag = "true".ExtractBool();

// Extract GUIDs
var guid = "550e8400-e29b-41d4-a716-446655440000".ExtractGuid();
```

#### Advanced Extraction with Configuration

```csharp
using Yash.FluentDataPipelines.Configuration;

// Extract using regex with group index
var config = new ExtractConfig
{
    RegexPattern = @"Price: \$(\d+\.\d{2})",
    GroupIndex = 1,  // Extract first capture group
    RegexOptions = RegexOptions.IgnoreCase
};

var price = "Price: $99.99".Extract<decimal>(config, (str, cfg) => 
    decimal.Parse(str, cfg.NumberStyles, cfg.Culture));

// Extract date with custom format
var dateConfig = new ExtractConfig
{
    DateTimeFormat = "dd/MM/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-GB")
};

var date = "25/12/2024".ExtractDate(dateConfig);
```

### Validation Operations

Validate values against conditions. Validations update the `IsValid` state but don't stop the pipeline.

#### Date Validations

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)           // Must be before now
    .After(DateTime.Now.AddDays(-7)); // Must be after 7 days ago
```

#### Numeric Validations

```csharp
var result = "42"
    .ExtractInt()
    .GreaterThan(10)              // Must be greater than 10
    .LessThan(100)                // Must be less than 100
    .Between(20, 50);             // Must be between 20 and 50 (inclusive)
```

#### String Validations

```csharp
var result = "user@example.com"
    .Contains("@")                 // Must contain @
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$"); // Must match email pattern
```

#### Custom Validation

```csharp
using Yash.FluentDataPipelines.Configuration;

var config = new ValidationConfig
{
    CustomValidator = value => ((int)value) % 2 == 0,
    ErrorMessage = "Value must be even"
};

var result = "42"
    .ExtractInt()
    .Validate(config, (value, cfg) => cfg.CustomValidator(value));
```

### Transformation Operations

Transform values while preserving validation state.

#### Date Transformations

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .AddDays(7)                   // Add 7 days
    .AddMonths(1)                 // Add 1 month
    .AddYears(1)                  // Add 1 year
    .Add(TimeSpan.FromHours(12)); // Add 12 hours
```

#### Numeric Transformations

```csharp
var result = "99.99"
    .ExtractDecimal()
    .Multiply(1.1m)               // Multiply by 1.1
    .Divide(2m)                   // Divide by 2
    .Round(2);                    // Round to 2 decimal places
```

#### String Transformations

```csharp
var result = "  Hello World  "
    .Trim()                       // Remove whitespace
    .ToUpper()                    // Convert to uppercase
    .ToLower()                    // Convert to lowercase
    .Replace("world", "Universe"); // Replace text
```

### Formatting Operations

Format values into human-readable strings with multiple options.

#### Basic Formatting

```csharp
// Default formatting
var result = "2024-12-10".ExtractDate().Format();

// Custom format string
var result = "2024-12-10".ExtractDate().Format("yyyy-MM-dd");

// Format with callback (value only)
var result = "42".ExtractInt().Format(value => $"Count: {value}");

// Format with callback (value + validation state)
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .Format((value, isValid) => isValid ? $"Valid: {value:yyyy-MM-dd}" : "Invalid date");
```

#### Type-Specific Formatting

```csharp
// Format dates
var dateStr = "2024-12-10".ExtractDate().FormatDate("yyyy-MM-dd");

// Format numbers
var numberStr = "99.99".ExtractDecimal().FormatNumber("N2");

// Format currency
var currencyStr = "99.99".ExtractDecimal().FormatCurrency();
```

### Collection Operations

Work with collections in your pipelines.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };

var result = numbers
    .Where(x => x > 2)            // Filter: [3, 4, 5]
    .Select(x => x * 2)           // Transform: [6, 8, 10]
    .First();                     // Get first: 6

// With default values
var firstOrDefault = numbers
    .Where(x => x > 10)
    .FirstOrDefault(0);          // Returns 0 if no match
```

## Configuration Reference

### ExtractConfig

Configure extraction operations with fine-grained control.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RegexPattern` | `string` | `null` | Regular expression pattern for extraction |
| `GroupIndex` | `int` | `0` | Regex group index to extract (0 = entire match) |
| `RegexOptions` | `RegexOptions` | `None` | Regex matching options |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for parsing dates/numbers |
| `DateTimeFormat` | `string` | `null` | Format string for DateTime parsing |
| `DateTimeFormats` | `string[]` | `null` | Array of format strings to try |
| `DateTimeStyles` | `DateTimeStyles` | `None` | DateTime parsing styles |
| `NumberStyles` | `NumberStyles` | `Any` | Number parsing styles |
| `ThrowOnFailure` | `bool` | `false` | Whether to throw exceptions on failure |

### ValidationConfig

Configure validation operations.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CaseSensitive` | `bool` | `true` | Whether string comparisons are case-sensitive |
| `StringComparison` | `StringComparison` | `Ordinal` | String comparison type |
| `Tolerance` | `double?` | `null` | Tolerance for numeric comparisons |
| `InclusiveLowerBound` | `bool` | `true` | Whether lower bound is inclusive in ranges |
| `InclusiveUpperBound` | `bool` | `true` | Whether upper bound is inclusive in ranges |
| `CustomValidator` | `Func<object, bool>` | `null` | Custom validation function |
| `ErrorMessage` | `string` | `null` | Custom error message for validation failures |

### TransformationConfig

Configure transformation operations.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RoundingMode` | `MidpointRounding` | `ToEven` | Rounding mode for numeric operations |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for transformations |
| `TrimWhitespace` | `bool` | `true` | Whether to trim whitespace |
| `TrimChars` | `char[]` | `null` | Specific characters to trim |
| `CustomTransform` | `Func<object, object>` | `null` | Custom transformation function |

### FormatConfig

Configure formatting operations.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FormatString` | `string` | `null` | Format string (e.g., "yyyy-MM-dd", "C") |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for formatting |
| `CustomFormatter` | `Func<object, string>` | `null` | Custom formatter function |
| `CustomFormatterWithValidation` | `Func<object, bool, string>` | `null` | Custom formatter with validation state |
| `NullValueString` | `string` | `""` | String to use for null values |
| `InvalidValueString` | `string` | `"Invalid"` | String to use for invalid values |

## Advanced Examples

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

### Chaining Multiple Validations

```csharp
var result = "user@example.com"
    .Contains("@")
    .Contains(".")
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
    .Format((value, isValid) => 
        isValid ? $"Valid email: {value}" : "Invalid email format");
```

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

