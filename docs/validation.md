# Validation Operations

Validation operations check if values meet certain conditions. Unlike traditional validation that throws exceptions, these operations update the `IsValid` state and continue execution, allowing you to collect all validation errors.

## Date Validations

### Before

Validates that a date is before a specified date.

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now);

// With custom error message
var config = new ValidationConfig
{
    ErrorMessage = "Date must be in the past"
};
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now, config);
```

### After

Validates that a date is after a specified date.

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .After(DateTime.Now.AddDays(-7));

// Check if date is within last week
var result = "2024-12-10"
    .ExtractDate()
    .After(DateTime.Now.AddDays(-7))
    .Before(DateTime.Now);
```

## Numeric Validations

### GreaterThan

Validates that a value is greater than another value.

```csharp
var result = "42"
    .ExtractInt()
    .GreaterThan(10);

// Works with any IComparable type
var result = "99.99"
    .ExtractDecimal()
    .GreaterThan(50m);
```

### LessThan

Validates that a value is less than another value.

```csharp
var result = "42"
    .ExtractInt()
    .LessThan(100);

var result = "99.99"
    .ExtractDecimal()
    .LessThan(200m);
```

### Between

Validates that a value is between two bounds (inclusive by default).

```csharp
// Inclusive bounds (default)
var result = "42"
    .ExtractInt()
    .Between(20, 50); // 20 <= value <= 50

// Exclusive lower bound
var config = new ValidationConfig
{
    InclusiveLowerBound = false
};
var result = "42"
    .ExtractInt()
    .Between(20, 50, config); // 20 < value <= 50

// Exclusive upper bound
var config = new ValidationConfig
{
    InclusiveUpperBound = false
};
var result = "42"
    .ExtractInt()
    .Between(20, 50, config); // 20 <= value < 50

// Both exclusive
var config = new ValidationConfig
{
    InclusiveLowerBound = false,
    InclusiveUpperBound = false
};
var result = "42"
    .ExtractInt()
    .Between(20, 50, config); // 20 < value < 50
```

### ApproximatelyEqual

Validates that a numeric value is approximately equal to another value within a tolerance.

```csharp
// With default tolerance (epsilon)
var result = "3.14159"
    .ExtractDouble()
    .ApproximatelyEqual(3.1416);

// With custom tolerance
var config = new ValidationConfig
{
    Tolerance = 0.01
};
var result = "3.14"
    .ExtractDouble()
    .ApproximatelyEqual(3.1416, config); // Within 0.01
```

## String Validations

### Contains

Validates that a string contains a specified substring.

```csharp
// Case-sensitive (default)
var result = "user@example.com"
    .Contains("@");

// Case-insensitive
var config = new ValidationConfig
{
    CaseSensitive = false
};
var result = "Hello World"
    .Contains("world", config); // Matches "World"

// Custom string comparison
var config = new ValidationConfig
{
    StringComparison = StringComparison.OrdinalIgnoreCase
};
var result = "Hello World"
    .Contains("world", config);
```

### Matches

Validates that a string matches a regular expression pattern.

```csharp
// Basic pattern matching
var result = "user@example.com"
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$");

// Case-insensitive matching
var config = new ValidationConfig
{
    CaseSensitive = false
};
var result = "User@Example.com"
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$", config);

// Complex validation
var result = "user@example.com"
    .Contains("@")
    .Contains(".")
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$");
```

## Custom Validation

Use the generic `Validate` method for custom validation logic.

### Simple Custom Validation

```csharp
var config = new ValidationConfig
{
    CustomValidator = value => ((int)value) % 2 == 0,
    ErrorMessage = "Value must be even"
};

var result = "42"
    .ExtractInt()
    .Validate(config, (value, cfg) => cfg.CustomValidator(value));
```

### Complex Custom Validation

```csharp
var config = new ValidationConfig
{
    ErrorMessage = "Value must be a valid credit card number"
};

var result = "1234-5678-9012-3456"
    .Validate(config, (value, cfg) =>
    {
        // Remove dashes and spaces
        string cleaned = value.Replace("-", "").Replace(" ", "");
        
        // Check length
        if (cleaned.Length != 16) return false;
        
        // Check if all digits
        if (!cleaned.All(char.IsDigit)) return false;
        
        // Luhn algorithm check
        return IsValidLuhn(cleaned);
    });
```

### Validation with Context

```csharp
var config = new ValidationConfig
{
    ErrorMessage = "Age must be between 18 and 65"
};

var result = "25"
    .ExtractInt()
    .Validate(config, (value, cfg) =>
    {
        int age = value;
        return age >= 18 && age <= 65;
    });
```

## Chaining Multiple Validations

You can chain multiple validations together. All validations are checked, and the final `IsValid` state reflects whether ALL validations passed.

```csharp
var result = "user@example.com"
    .Contains("@")
    .Contains(".")
    .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
    .Format((value, isValid) => 
        isValid ? $"Valid email: {value}" : "Invalid email format");

// All validations must pass
var result = "42"
    .ExtractInt()
    .GreaterThan(10)
    .LessThan(100)
    .Between(20, 50);
```

## Validation State

### Checking Validation Results

```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .After(DateTime.Now.AddDays(-7));

if (result.IsValid)
{
    Console.WriteLine($"Valid date: {result.Value}");
}
else
{
    Console.WriteLine("Validation failed!");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  {error.Operation}: {error.Message}");
    }
}
```

### Validation State Propagation

Validation failures don't stop the pipeline. The invalid state propagates through subsequent operations:

```csharp
var result = "invalid-date"
    .ExtractDate()           // Fails: IsValid = false
    .Before(DateTime.Now)    // Still executes, adds another error
    .After(DateTime.Now.AddDays(-7)); // Still executes

// result.IsValid will be false
// result.Errors will contain errors from ExtractDate and Before
```

## Error Messages

### Default Error Messages

Each validation operation has a default error message:

```csharp
var result = "42"
    .ExtractInt()
    .GreaterThan(50); // Default error: "Validation failed"
```

### Custom Error Messages

Provide custom error messages for better debugging:

```csharp
var config = new ValidationConfig
{
    ErrorMessage = "Value must be greater than 50"
};

var result = "42"
    .ExtractInt()
    .GreaterThan(50, config);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error.Message); // "Value must be greater than 50"
    }
}
```

## Best Practices

1. **Chain related validations**: Group related validations together for clarity.

2. **Use descriptive error messages**: Custom error messages help identify issues quickly.

3. **Check validation state**: Always verify `IsValid` before using values in critical operations.

4. **Review error collection**: Use the `Errors` collection to understand all validation failures.

5. **Use appropriate comparison types**: Choose `CaseSensitive` and `StringComparison` based on your needs.

6. **Set tolerance for floating-point**: Use `ApproximatelyEqual` with appropriate tolerance for floating-point comparisons.

7. **Validate early**: Perform validations as early as possible in the pipeline to catch issues quickly.

8. **Combine with transformations**: Validations work well with transformations to ensure data quality.

