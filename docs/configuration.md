# Configuration Reference

This document provides a complete reference for all configuration objects in the FluentDataPipelines library.

## ExtractConfig

Configures extraction operations from strings.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RegexPattern` | `string` | `null` | Regular expression pattern for extraction |
| `GroupIndex` | `int` | `0` | Regex group index to extract (0 = entire match, 1 = first group, etc.) |
| `RegexOptions` | `RegexOptions` | `None` | Regex matching options (IgnoreCase, Multiline, etc.) |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for parsing dates and numbers |
| `DateTimeFormat` | `string` | `null` | Format string for DateTime parsing (e.g., "yyyy-MM-dd") |
| `DateTimeFormats` | `string[]` | `null` | Array of format strings to try in order |
| `DateTimeStyles` | `DateTimeStyles` | `None` | DateTime parsing styles (AllowWhiteSpaces, etc.) |
| `NumberStyles` | `NumberStyles` | `Any` | Number parsing styles (Currency, AllowDecimalPoint, etc.) |
| `ThrowOnFailure` | `bool` | `false` | Whether to throw exceptions on failure (default: returns invalid PipelineValue) |
| `UseDefaultRegex` | `bool` | `true` | Whether to use default regex patterns when no custom pattern provided |
| `FuzzyMatchingConfig` | `FuzzyMatchingConfig` | `null` | Configuration for fuzzy extraction |
| `FuzzyExtractionMode` | `FuzzyExtractionMode` | `None` | Mode for fuzzy extraction (None, Fallback, Primary) |

### Default Regex Patterns

The library includes default regex patterns for common types:

- **Date**: `\d{1,4}[-\/]\d{1,2}[-\/]\d{1,4}|\d{4}-\d{2}-\d{2}|...`
- **Int**: `-?\d+`
- **Double/Decimal**: `-?\d+\.?\d*`
- **Bool**: `(?i)true|false|yes|no|1|0`
- **Guid**: `[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-...`

### Example

```csharp
var config = new ExtractConfig
{
    RegexPattern = @"Price: \$(\d+\.\d{2})",
    GroupIndex = 1,
    RegexOptions = RegexOptions.IgnoreCase,
    Culture = CultureInfo.GetCultureInfo("en-US"),
    NumberStyles = NumberStyles.Currency,
    ThrowOnFailure = false,
    UseDefaultRegex = true
};
```

## ValidationConfig

Configures validation operations.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `CaseSensitive` | `bool` | `true` | Whether string comparisons are case-sensitive |
| `StringComparison` | `StringComparison` | `Ordinal` | String comparison type (Ordinal, OrdinalIgnoreCase, etc.) |
| `Tolerance` | `double?` | `null` | Tolerance for numeric comparisons (for ApproximatelyEqual) |
| `InclusiveLowerBound` | `bool` | `true` | Whether lower bound is inclusive in range validations |
| `InclusiveUpperBound` | `bool` | `true` | Whether upper bound is inclusive in range validations |
| `CustomValidator` | `Func<object, bool>` | `null` | Custom validation function |
| `ErrorMessage` | `string` | `null` | Custom error message for validation failures |

### Example

```csharp
var config = new ValidationConfig
{
    CaseSensitive = false,
    StringComparison = StringComparison.OrdinalIgnoreCase,
    Tolerance = 0.01,
    InclusiveLowerBound = true,
    InclusiveUpperBound = false,
    ErrorMessage = "Value is out of range"
};
```

## TransformationConfig

Configures transformation operations.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RoundingMode` | `MidpointRounding` | `ToEven` | Rounding mode for numeric operations (ToEven, AwayFromZero, etc.) |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for transformations (formatting, parsing, etc.) |
| `TrimWhitespace` | `bool` | `true` | Whether to trim whitespace in string transformations |
| `TrimChars` | `char[]` | `null` | Specific characters to trim (if null, trims all whitespace) |
| `CustomTransform` | `Func<object, object>` | `null` | Custom transformation function |

### Rounding Modes

- **`ToEven`** (default): Rounds to nearest even number (banker's rounding)
- **`AwayFromZero`**: Rounds away from zero
- **`ToZero`**: Rounds toward zero
- **`ToNegativeInfinity`**: Rounds down
- **`ToPositiveInfinity`**: Rounds up

### Example

```csharp
var config = new TransformationConfig
{
    RoundingMode = MidpointRounding.AwayFromZero,
    Culture = CultureInfo.GetCultureInfo("en-US"),
    TrimWhitespace = true,
    TrimChars = new[] { '-', '_' }
};
```

## FormatConfig

Configures formatting operations.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FormatString` | `string` | `null` | Format string (e.g., "yyyy-MM-dd" for dates, "C" for currency) |
| `Culture` | `CultureInfo` | `CurrentCulture` | Culture for formatting |
| `CustomFormatter` | `Func<object, string>` | `null` | Custom formatter function (takes value, returns string) |
| `CustomFormatterWithValidation` | `Func<object, bool, string>` | `null` | Custom formatter with validation state (takes value and isValid, returns string) |
| `NullValueString` | `string` | `""` | String to use for null values |
| `InvalidValueString` | `string` | `"Invalid"` | String to use for invalid values |

### Priority

When multiple formatters are specified, priority is:
1. `CustomFormatterWithValidation` (highest)
2. `CustomFormatter`
3. `FormatString`
4. Default `ToString()` (lowest)

### Example

```csharp
var config = new FormatConfig
{
    FormatString = "yyyy-MM-dd",
    Culture = CultureInfo.GetCultureInfo("en-US"),
    NullValueString = "N/A",
    InvalidValueString = "Invalid",
    CustomFormatterWithValidation = (value, isValid) =>
        isValid ? $"✅ {value}" : $"❌ Invalid"
};
```

## FuzzyMatchingConfig

Configures fuzzy matching operations.

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Algorithm` | `FuzzyAlgorithm` | `JaroWinkler` | Fuzzy matching algorithm (Levenshtein, Jaro, JaroWinkler) |
| `SimilarityThreshold` | `double` | `0.8` | Similarity threshold (0.0 to 1.0) required for a match |
| `CaseSensitive` | `bool` | `false` | Whether string comparisons are case-sensitive |
| `MaxEditDistance` | `int?` | `null` | Maximum edit distance allowed (for Levenshtein algorithm only) |
| `NormalizeAddress` | `bool` | `false` | Whether to normalize addresses before comparison |
| `NormalizePhone` | `bool` | `false` | Whether to normalize phone numbers before comparison |
| `NormalizeName` | `bool` | `false` | Whether to normalize names before comparison |
| `CustomSimilarityFunction` | `Func<string, string, double>` | `null` | Custom similarity function (takes two strings, returns 0.0 to 1.0) |
| `ErrorMessage` | `string` | `null` | Custom error message for fuzzy matching failures |
| `ReturnBestMatch` | `bool` | `false` | Whether to return best match even if below threshold (for batch operations) |

### FuzzyAlgorithm Enum

- **`Levenshtein`**: Edit distance-based similarity
- **`Jaro`**: Jaro similarity algorithm
- **`JaroWinkler`**: Jaro-Winkler similarity (default, optimized for names)

### Example

```csharp
var config = new FuzzyMatchingConfig
{
    Algorithm = FuzzyAlgorithm.JaroWinkler,
    SimilarityThreshold = 0.8,
    CaseSensitive = false,
    MaxEditDistance = 3,
    NormalizeAddress = true,
    NormalizePhone = false,
    NormalizeName = true,
    ErrorMessage = "Strings do not match",
    ReturnBestMatch = false
};
```

## FuzzyExtractionMode Enum

Defines the mode for fuzzy extraction.

- **`None`**: No fuzzy matching (default, backward compatible)
- **`Fallback`**: Try strict extraction first, use fuzzy if it fails
- **`Primary`**: Use fuzzy matching to find best candidate from regex matches

## Default Configurations

All configuration classes provide a `Default` static property for convenience:

```csharp
// Use default configuration
var config = ExtractConfig.Default;
var config = ValidationConfig.Default;
var config = TransformationConfig.Default;
var config = FormatConfig.Default;
var config = FuzzyMatchingConfig.Default;
```

## Configuration Best Practices

1. **Reuse configurations**: Create configuration objects once and reuse them for similar operations.

2. **Use defaults when possible**: Start with default configurations and customize only what you need.

3. **Set appropriate cultures**: Always set the correct `Culture` for locale-specific operations.

4. **Configure error messages**: Provide meaningful error messages for better debugging.

5. **Test configurations**: Test your configurations with sample data to ensure they work as expected.

6. **Document custom functions**: If using custom validators, transformers, or formatters, document their behavior.

7. **Consider performance**: Some configurations (like fuzzy matching) can be expensive. Use them judiciously.

8. **Validate configurations**: Ensure configuration values are within valid ranges (e.g., SimilarityThreshold 0.0-1.0).

