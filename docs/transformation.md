# Transformation Operations

Transformation operations modify values while preserving validation state. Transformations on invalid values typically become no-ops, maintaining the invalid state throughout the pipeline.

## Date Transformations

### AddDays

Adds days to a date. Supports both integer and fractional days.

```csharp
// Add integer days
var result = "2024-12-10"
    .ExtractDate()
    .AddDays(7);

// Add fractional days
var result = "2024-12-10"
    .ExtractDate()
    .AddDays(7.5); // Adds 7 days and 12 hours
```

### AddMonths

Adds months to a date.

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .AddMonths(1); // 2025-01-10
```

### AddYears

Adds years to a date.

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .AddYears(1); // 2025-12-10
```

### Add (TimeSpan)

Adds a `TimeSpan` to a date.

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Add(TimeSpan.FromHours(12)); // Add 12 hours

var result = "2024-12-10"
    .ExtractDate()
    .Add(TimeSpan.FromDays(7).Add(TimeSpan.FromHours(6))); // Add 7 days and 6 hours
```

### Chaining Date Transformations

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .AddDays(7)        // Add 7 days
    .AddMonths(1)      // Add 1 month
    .AddYears(1)       // Add 1 year
    .Add(TimeSpan.FromHours(12)); // Add 12 hours
```

## Numeric Transformations

### Multiply

Multiplies a numeric value by a factor.

```csharp
// Integer multiplication
var result = "42"
    .ExtractInt()
    .Multiply(2); // 84

// Double multiplication
var result = "99.99"
    .ExtractDouble()
    .Multiply(1.1); // 109.989

// Decimal multiplication (preferred for financial calculations)
var result = "99.99"
    .ExtractDecimal()
    .Multiply(1.15m); // 114.9885 (add 15% tax)
```

### Divide

Divides a numeric value by a divisor.

```csharp
// Integer division
var result = "42"
    .ExtractInt()
    .Divide(2); // 21

// Double division
var result = "99.99"
    .ExtractDouble()
    .Divide(2.0); // 49.995

// Decimal division
var result = "99.99"
    .ExtractDecimal()
    .Divide(2m); // 49.995
```

### Round

Rounds a numeric value to a specified number of decimal places.

```csharp
// Round double
var result = "99.98765"
    .ExtractDouble()
    .Round(2); // 99.99

// Round decimal
var result = "99.98765"
    .ExtractDecimal()
    .Round(2); // 99.99

// Custom rounding mode
var config = new TransformationConfig
{
    RoundingMode = MidpointRounding.AwayFromZero
};
var result = "99.985"
    .ExtractDecimal()
    .Round(2, config); // 99.99 (rounds up)
```

### Chaining Numeric Transformations

```csharp
// Calculate price with tax and discount
var result = "99.99"
    .ExtractDecimal()
    .Multiply(1.15m)  // Add 15% tax
    .Multiply(0.9m)    // Apply 10% discount
    .Round(2);        // Round to 2 decimal places
```

## String Transformations

### Trim

Removes whitespace from the beginning and end of a string.

```csharp
// Basic trim
var result = "  Hello World  "
    .Trim(); // "Hello World"

// Trim specific characters
var config = new TransformationConfig
{
    TrimChars = new[] { '-', '_' }
};
var result = "--Hello World--"
    .Trim(config); // "Hello World"
```

### ToUpper / ToLower

Converts a string to uppercase or lowercase.

```csharp
// To uppercase
var result = "Hello World"
    .ToUpper(); // "HELLO WORLD"

// To lowercase
var result = "Hello World"
    .ToLower(); // "hello world"

// With culture
var config = new TransformationConfig
{
    Culture = CultureInfo.GetCultureInfo("tr-TR") // Turkish culture
};
var result = "i"
    .ToUpper(config); // "İ" (Turkish uppercase i)
```

### Replace

Replaces occurrences of a string with another string.

```csharp
var result = "Hello World"
    .Replace("World", "Universe"); // "Hello Universe"

// Multiple replacements (chain them)
var result = "Hello World"
    .Replace("Hello", "Hi")
    .Replace("World", "Universe"); // "Hi Universe"
```

### Chaining String Transformations

```csharp
var result = "  Hello World  "
    .Trim()                    // "Hello World"
    .ToUpper()                  // "HELLO WORLD"
    .Replace("WORLD", "Universe"); // "HELLO Universe"
```

## Custom Transformations

Use the generic `Transform` method for custom transformation logic.

### String Transformations

```csharp
var config = new TransformationConfig();

var result = "hello world"
    .Transform(config, (value, cfg) =>
    {
        // Capitalize first letter of each word
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
    }); // "Hello World"
```

### Numeric Transformations

```csharp
var config = new TransformationConfig();

var result = "42"
    .ExtractInt()
    .Transform(config, (value, cfg) =>
    {
        // Calculate factorial
        int factorial = 1;
        for (int i = 2; i <= value; i++)
        {
            factorial *= i;
        }
        return factorial;
    }); // 1405006117752879898543142606244511569936384000000000
```

### Date Transformations

```csharp
var config = new TransformationConfig();

var result = "2024-12-10"
    .ExtractDate()
    .Transform(config, (value, cfg) =>
    {
        // Get first day of month
        return new DateTime(value.Year, value.Month, 1);
    }); // 2024-12-01
```

## Transformation with Validation State

Transformations respect validation state. If a value is invalid, transformations typically become no-ops:

```csharp
var result = "invalid-date"
    .ExtractDate()      // Fails: IsValid = false
    .AddDays(7);        // No-op: preserves invalid state

// result.IsValid is still false
// result.Value is still the default DateTime
```

However, you can still access the value for conditional transformations:

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Transform(config, (value, cfg) =>
    {
        // Transform only if valid
        if (value.Year < 2020)
        {
            return value.AddYears(1);
        }
        return value;
    });
```

## Configuration

### TransformationConfig

Control transformation behavior with `TransformationConfig`:

```csharp
var config = new TransformationConfig
{
    RoundingMode = MidpointRounding.AwayFromZero, // For rounding operations
    Culture = CultureInfo.GetCultureInfo("en-US"), // For culture-specific operations
    TrimWhitespace = true,                        // For string operations
    TrimChars = new[] { '-', '_' }                // Specific characters to trim
};
```

### Rounding Modes

- **`ToEven`** (default): Rounds to nearest even number (banker's rounding)
- **`AwayFromZero`**: Rounds away from zero
- **`ToZero`**: Rounds toward zero
- **`ToNegativeInfinity`**: Rounds down
- **`ToPositiveInfinity`**: Rounds up

```csharp
var config = new TransformationConfig
{
    RoundingMode = MidpointRounding.AwayFromZero
};

var result = "2.5"
    .ExtractDecimal()
    .Round(0, config); // 3 (rounds away from zero)
```

## Best Practices

1. **Chain related transformations**: Group related transformations together for clarity.

2. **Preserve precision**: Use `decimal` for financial calculations, `double` for scientific calculations.

3. **Round at the end**: Apply rounding as the last step to preserve precision during calculations.

4. **Use appropriate cultures**: Set the correct `Culture` for locale-specific transformations.

5. **Check validation state**: Verify `IsValid` before using transformed values in critical operations.

6. **Transform before validation**: Apply transformations before validations to ensure data is in the correct format.

7. **Use custom transformations for complex logic**: For complex transformations, use the generic `Transform` method.

8. **Consider performance**: Chain transformations efficiently to minimize intermediate allocations.

