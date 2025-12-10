# Formatting Operations

Formatting operations convert `PipelineValue<T>` instances into human-readable strings. The library provides multiple formatting options, from simple default formatting to complex custom formatters.

## Basic Formatting

### Default Format

Uses the default `ToString()` method of the value type.

```csharp
// Default formatting
var result = "2024-12-10"
    .ExtractDate()
    .Format(); // Uses DateTime.ToString()

var result = "42"
    .ExtractInt()
    .Format(); // "42"

var result = "99.99"
    .ExtractDecimal()
    .Format(); // "99.99"
```

### Format String

Uses a format string (standard .NET format strings).

```csharp
// Date formatting
var result = "2024-12-10"
    .ExtractDate()
    .Format("yyyy-MM-dd"); // "2024-12-10"

var result = "2024-12-10"
    .ExtractDate()
    .Format("dd/MM/yyyy"); // "10/12/2024"

// Number formatting
var result = "99.99"
    .ExtractDecimal()
    .Format("N2"); // "99.99" (number with 2 decimal places)

var result = "1234.56"
    .ExtractDecimal()
    .Format("C"); // "$1,234.56" (currency format)
```

## Type-Specific Formatting

### FormatDate

Formats a `DateTime` value as a date string.

```csharp
// Default format (short date)
var result = "2024-12-10"
    .ExtractDate()
    .FormatDate(); // Uses short date format

// Custom format
var result = "2024-12-10"
    .ExtractDate()
    .FormatDate("yyyy-MM-dd"); // "2024-12-10"

var result = "2024-12-10"
    .ExtractDate()
    .FormatDate("dd MMM yyyy"); // "10 Dec 2024"
```

### FormatNumber

Formats a numeric value as a number string.

```csharp
// Default format (N2 - number with 2 decimal places)
var result = "99.99"
    .ExtractDecimal()
    .FormatNumber(); // "99.99"

// Custom format
var result = "1234.5678"
    .ExtractDecimal()
    .FormatNumber("N4"); // "1,234.5678"

var result = "1234.56"
    .ExtractDecimal()
    .FormatNumber("F2"); // "1234.56" (fixed point, 2 decimals)
```

### FormatCurrency

Formats a numeric value as currency.

```csharp
// Default currency format (current culture)
var result = "99.99"
    .ExtractDecimal()
    .FormatCurrency(); // "$99.99" (US culture)

// With specific culture
var result = "99.99"
    .ExtractDecimal()
    .FormatCurrency(CultureInfo.GetCultureInfo("en-GB")); // "£99.99"

var result = "99.99"
    .ExtractDecimal()
    .FormatCurrency(CultureInfo.GetCultureInfo("de-DE")); // "99,99 €"
```

## Custom Formatting

### Format with Value Only

Use a function that takes only the value:

```csharp
var result = "42"
    .ExtractInt()
    .Format(value => $"Count: {value}"); // "Count: 42"

var result = "2024-12-10"
    .ExtractDate()
    .Format(value => value.ToString("yyyy-MM-dd HH:mm:ss")); // "2024-12-10 00:00:00"
```

### Format with Validation State

Use a function that takes both the value and validation state:

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .Format((value, isValid) => 
        isValid ? $"Valid date: {value:yyyy-MM-dd}" : "Invalid date");

var result = "42"
    .ExtractInt()
    .GreaterThan(50)
    .Format((value, isValid) => 
        isValid ? $"Value: {value}" : $"Invalid value: {value} (must be > 50)");
```

### Complex Custom Formatting

```csharp
var result = "99.99"
    .ExtractDecimal()
    .Format((value, isValid) =>
    {
        if (!isValid)
        {
            return "N/A";
        }
        
        if (value < 0)
        {
            return $"Loss: {value:C}";
        }
        else if (value > 1000)
        {
            return $"High: {value:C}";
        }
        else
        {
            return $"Normal: {value:C}";
        }
    });
```

## Format Configuration

### FormatConfig

Control formatting behavior with `FormatConfig`:

```csharp
var config = new FormatConfig
{
    FormatString = "yyyy-MM-dd",
    Culture = CultureInfo.GetCultureInfo("en-US"),
    NullValueString = "N/A",
    InvalidValueString = "Invalid"
};

var result = "2024-12-10"
    .ExtractDate()
    .Format(config);
```

### Null and Invalid Values

Configure how null and invalid values are formatted:

```csharp
var config = new FormatConfig
{
    NullValueString = "No value",
    InvalidValueString = "Invalid value"
};

// Null value
var result = new PipelineValue<string>(null, isValid: true);
string formatted = result.Format(config); // "No value"

// Invalid value
var result = "invalid-date"
    .ExtractDate();
string formatted = result.Format(config); // "Invalid value"
```

### Custom Formatters

Use custom formatter functions for complex formatting logic:

```csharp
var config = new FormatConfig
{
    CustomFormatter = value =>
    {
        if (value is DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        return value?.ToString() ?? "N/A";
    }
};

var result = "2024-12-10"
    .ExtractDate()
    .Format(config);
```

### Formatter with Validation State

Use a formatter that considers validation state:

```csharp
var config = new FormatConfig
{
    CustomFormatterWithValidation = (value, isValid) =>
    {
        if (!isValid)
        {
            return "❌ Invalid";
        }
        
        if (value is DateTime dt)
        {
            return $"✅ {dt:yyyy-MM-dd}";
        }
        
        return value?.ToString() ?? "N/A";
    }
};

var result = "2024-12-10"
    .ExtractDate()
    .Format(config);
```

## Common Format Strings

### Date Format Strings

- `"d"`: Short date (e.g., "12/10/2024")
- `"D"`: Long date (e.g., "Tuesday, December 10, 2024")
- `"yyyy-MM-dd"`: ISO format (e.g., "2024-12-10")
- `"dd/MM/yyyy"`: European format (e.g., "10/12/2024")
- `"MMM dd, yyyy"`: Month name format (e.g., "Dec 10, 2024")

### Number Format Strings

- `"N"` or `"N2"`: Number with thousands separator and decimals (e.g., "1,234.56")
- `"F"` or `"F2"`: Fixed point format (e.g., "1234.56")
- `"C"` or `"C2"`: Currency format (e.g., "$1,234.56")
- `"P"` or `"P2"`: Percent format (e.g., "12.34%")
- `"E"` or `"E2"`: Exponential format (e.g., "1.23E+003")

## Culture-Specific Formatting

### Date Formatting

```csharp
// US format
var config = new FormatConfig
{
    FormatString = "MM/dd/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var result = "2024-12-10"
    .ExtractDate()
    .Format(config); // "12/10/2024"

// UK format
var config = new FormatConfig
{
    FormatString = "dd/MM/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-GB")
};
var result = "2024-12-10"
    .ExtractDate()
    .Format(config); // "10/12/2024"
```

### Number Formatting

```csharp
// US numbers
var config = new FormatConfig
{
    FormatString = "N2",
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var result = "1234.56"
    .ExtractDecimal()
    .Format(config); // "1,234.56"

// German numbers
var config = new FormatConfig
{
    FormatString = "N2",
    Culture = CultureInfo.GetCultureInfo("de-DE")
};
var result = "1234.56"
    .ExtractDecimal()
    .Format(config); // "1.234,56"
```

## Formatting in Pipelines

### Complete Pipeline Example

```csharp
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
    .Format((value, isValid) => 
        isValid ? $"Final price: {value:C}" : "Invalid price");
```

### Conditional Formatting

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .Format((value, isValid) =>
    {
        if (!isValid)
        {
            return "Date validation failed";
        }
        
        var daysAgo = (DateTime.Now - value).Days;
        return $"Date was {daysAgo} days ago";
    });
```

## Best Practices

1. **Use format strings for standard formats**: Leverage standard .NET format strings for common formatting needs.

2. **Consider validation state**: Use formatters that take validation state when you need to handle invalid values differently.

3. **Set appropriate cultures**: Use the correct `Culture` for locale-specific formatting.

4. **Handle null and invalid values**: Configure `NullValueString` and `InvalidValueString` for better user experience.

5. **Format at the end**: Apply formatting as the last step in your pipeline.

6. **Use type-specific methods**: Use `FormatDate`, `FormatNumber`, and `FormatCurrency` for better type safety.

7. **Custom formatters for complex logic**: Use custom formatters for complex formatting requirements.

8. **Test with different cultures**: Ensure your formatting works correctly with different culture settings.

