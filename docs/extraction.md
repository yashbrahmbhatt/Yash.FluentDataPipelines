# Extraction Operations

Extraction operations convert strings into typed values. The library provides both simple convenience methods and advanced configuration options for complex scenarios.

## Basic Extraction Methods

### ExtractDate

Extracts a `DateTime` from a string.

```csharp
// Simple extraction (uses default culture and formats)
var date = "2024-12-10".ExtractDate();

// With custom format
var config = new ExtractConfig
{
    DateTimeFormat = "dd/MM/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-GB")
};
var date = "25/12/2024".ExtractDate(config);

// With multiple format options
var config = new ExtractConfig
{
    DateTimeFormats = new[] { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" },
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var date = "12/25/2024".ExtractDate(config);
```

### ExtractInt

Extracts an `int` from a string.

```csharp
// Simple extraction
var number = "42".ExtractInt();

// With culture-specific formatting
var config = new ExtractConfig
{
    Culture = CultureInfo.GetCultureInfo("de-DE") // Handles German number formats
};
var number = "1.234".ExtractInt(config); // Parses as 1234 (German uses . as thousands separator)
```

### ExtractDouble / ExtractDecimal

Extract floating-point numbers from strings.

```csharp
// Extract double
var price = "99.99".ExtractDouble();

// Extract decimal (preferred for financial calculations)
var price = "99.99".ExtractDecimal();

// With number styles
var config = new ExtractConfig
{
    NumberStyles = NumberStyles.Currency,
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var price = "$99.99".ExtractDecimal(config);
```

### ExtractBool

Extracts a `bool` from a string. Handles common boolean representations.

```csharp
// Standard boolean values
"true".ExtractBool();   // true
"false".ExtractBool();  // false

// Alternative representations
"1".ExtractBool();      // true
"0".ExtractBool();     // false
"yes".ExtractBool();   // true
"no".ExtractBool();    // false
"y".ExtractBool();     // true
"n".ExtractBool();     // false
```

### ExtractGuid

Extracts a `Guid` from a string.

```csharp
var guid = "550e8400-e29b-41d4-a716-446655440000".ExtractGuid();
```

## Advanced Extraction with Regex

Use regular expressions to extract values from complex strings.

### Basic Regex Extraction

```csharp
var config = new ExtractConfig
{
    RegexPattern = @"Price: \$(\d+\.\d{2})",
    GroupIndex = 1  // Extract first capture group
};

var price = "Product: Widget, Price: $99.99"
    .Extract<decimal>(config, (str, cfg) => 
        decimal.Parse(str, cfg.NumberStyles, cfg.Culture));
```

### Regex Options

```csharp
var config = new ExtractConfig
{
    RegexPattern = @"price:\s*\$?(\d+\.\d{2})",
    GroupIndex = 1,
    RegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline
};

var price = "PRICE: $99.99".Extract<decimal>(config, 
    (str, cfg) => decimal.Parse(str, cfg.NumberStyles, cfg.Culture));
```

### Multiple Groups

```csharp
var config = new ExtractConfig
{
    RegexPattern = @"(\d{4})-(\d{2})-(\d{2})",
    GroupIndex = 0  // Entire match: "2024-12-10"
};

// Or extract specific groups
var config = new ExtractConfig
{
    RegexPattern = @"(\d{4})-(\d{2})-(\d{2})",
    GroupIndex = 1  // Year: "2024"
};
```

## Default Regex Patterns

The library includes default regex patterns for common extraction types. These are used automatically when `UseDefaultRegex` is `true` (default).

### Available Default Patterns

- **Date**: `\d{1,4}[-\/]\d{1,2}[-\/]\d{1,4}|\d{4}-\d{2}-\d{2}|...`
- **Int**: `-?\d+`
- **Double/Decimal**: `-?\d+\.?\d*`
- **Bool**: `(?i)true|false|yes|no|1|0`
- **Guid**: `[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-...`

### Using Default Patterns

```csharp
// Default patterns are used automatically
var date = "The date is 2024-12-10".ExtractDate(); // Uses default date pattern

// Disable default patterns
var config = new ExtractConfig
{
    UseDefaultRegex = false,
    RegexPattern = @"\d{4}-\d{2}-\d{2}"  // Custom pattern only
};
var date = "2024-12-10".ExtractDate(config);
```

## Fuzzy Extraction

Fuzzy extraction helps when data is slightly malformed or contains typos. It uses fuzzy matching algorithms to find the best candidate from regex matches.

### Fuzzy Extraction Modes

#### None (Default)

No fuzzy matching. Standard extraction behavior.

```csharp
var config = new ExtractConfig
{
    FuzzyExtractionMode = FuzzyExtractionMode.None
};
```

#### Fallback

Try strict extraction first, use fuzzy matching if parsing fails.

```csharp
var config = new ExtractConfig
{
    FuzzyExtractionMode = FuzzyExtractionMode.Fallback,
    FuzzyMatchingConfig = new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.8,
        Algorithm = FuzzyAlgorithm.JaroWinkler
    },
    RegexPattern = @"\d{4}-\d{2}-\d{2}"
};

// Will try strict parsing first, then fuzzy if it fails
var date = "2024-12-1O".ExtractDate(config); // "O" instead of "0" - fuzzy helps
```

#### Primary

Use fuzzy matching to find the best candidate from multiple regex matches.

```csharp
var config = new ExtractConfig
{
    FuzzyExtractionMode = FuzzyExtractionMode.Primary,
    FuzzyMatchingConfig = new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.7,
        Algorithm = FuzzyAlgorithm.Levenshtein
    },
    RegexPattern = @"\d{4}-\d{2}-\d{2}"
};

// If multiple matches found, fuzzy matching selects the best one
var date = "Dates: 2024-12-10, 2024-12-1O, 2024-12-20".ExtractDate(config);
```

### Fuzzy Extraction Example

```csharp
var config = new ExtractConfig
{
    FuzzyExtractionMode = FuzzyExtractionMode.Fallback,
    FuzzyMatchingConfig = new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.8,
        Algorithm = FuzzyAlgorithm.JaroWinkler
    },
    DateTimeFormat = "yyyy-MM-dd"
};

// Handles typos and malformed dates
var date1 = "2024-12-1O".ExtractDate(config);  // Typo: O instead of 0
var date2 = "2024-12-1".ExtractDate(config);    // Missing digit
```

## Custom Extraction

For complex scenarios, use the generic `Extract<T>` method with a custom converter.

### Basic Custom Extraction

```csharp
var config = new ExtractConfig
{
    RegexPattern = @"ID: (\w+)"
};

var id = "ID: ABC123"
    .Extract<string>(config, (str, cfg) => str.ToUpper());
```

### Complex Type Extraction

```csharp
// Extract a custom type
public class ProductInfo
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

var config = new ExtractConfig
{
    RegexPattern = @"Product: (\w+), Price: \$(\d+\.\d{2})",
    GroupIndex = 0  // Entire match
};

var product = "Product: Widget, Price: $99.99"
    .Extract<ProductInfo>(config, (str, cfg) =>
    {
        var match = Regex.Match(str, @"Product: (\w+), Price: \$(\d+\.\d{2})");
        return new ProductInfo
        {
            Name = match.Groups[1].Value,
            Price = decimal.Parse(match.Groups[2].Value, cfg.NumberStyles, cfg.Culture)
        };
    });
```

## Error Handling

### Throw on Failure

By default, extraction failures return invalid `PipelineValue` instances. You can configure extraction to throw exceptions instead.

```csharp
var config = new ExtractConfig
{
    ThrowOnFailure = true,
    RegexPattern = @"\d+"
};

try
{
    var number = "abc".ExtractInt(config); // Throws exception
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Extraction failed: {ex.Message}");
}
```

### Check Validation State

```csharp
var date = "invalid-date".ExtractDate();

if (!date.IsValid)
{
    Console.WriteLine("Extraction failed!");
    foreach (var error in date.Errors)
    {
        Console.WriteLine($"  {error.Operation}: {error.Message}");
    }
}
```

## Culture and Localization

### Date Formats

```csharp
// US format
var configUS = new ExtractConfig
{
    DateTimeFormat = "MM/dd/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var dateUS = "12/25/2024".ExtractDate(configUS);

// UK format
var configUK = new ExtractConfig
{
    DateTimeFormat = "dd/MM/yyyy",
    Culture = CultureInfo.GetCultureInfo("en-GB")
};
var dateUK = "25/12/2024".ExtractDate(configUK);
```

### Number Formats

```csharp
// US numbers
var configUS = new ExtractConfig
{
    Culture = CultureInfo.GetCultureInfo("en-US")
};
var numberUS = "1,234.56".ExtractDecimal(configUS); // 1234.56

// German numbers
var configDE = new ExtractConfig
{
    Culture = CultureInfo.GetCultureInfo("de-DE")
};
var numberDE = "1.234,56".ExtractDecimal(configDE); // 1234.56
```

## Best Practices

1. **Use specific formats when possible**: If you know the exact format, specify it for better performance and accuracy.

2. **Handle multiple formats**: Use `DateTimeFormats` array when data may come in different formats.

3. **Consider fuzzy extraction for messy data**: When dealing with OCR or user input, fuzzy extraction can help.

4. **Check validation state**: Always verify `IsValid` before using extracted values.

5. **Use appropriate cultures**: Set the correct `Culture` for locale-specific parsing.

6. **Leverage default patterns**: For common cases, default regex patterns work well and reduce configuration.

7. **Custom extraction for complex types**: Use the generic `Extract<T>` method for extracting into custom types.

