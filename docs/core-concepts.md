# Core Concepts

This document explains the fundamental concepts of the FluentDataPipelines library.

## PipelineValue<T>

The heart of the library is `PipelineValue<T>`, an immutable wrapper that tracks the value being processed along with its validation state.

### Properties

- **`Value`** (`T`): The actual data being processed
- **`IsValid`** (`bool`): Whether the value has passed all validations
- **`Errors`** (`IReadOnlyList<PipelineError>`): Collection of errors that occurred during processing

### Creating PipelineValues

```csharp
// Create a valid PipelineValue
var validValue = new PipelineValue<string>("Hello World", isValid: true);

// Create an invalid PipelineValue with an error
var invalidValue = new PipelineValue<int>(0, isValid: false, 
    errors: new[] { new PipelineError("Value must be positive", "Validation") });

// Implicit conversion from value to PipelineValue
PipelineValue<string> pv = "Hello"; // Automatically creates a valid PipelineValue
```

### Immutability

All operations on `PipelineValue<T>` return new instances. The original value is never modified, ensuring thread safety and predictable behavior.

```csharp
var original = new PipelineValue<string>("Hello");
var modified = original.Trim(); // Returns a NEW PipelineValue

Console.WriteLine(original.Value); // Still "Hello"
Console.WriteLine(modified.Value); // "Hello" (trimmed, but original unchanged)
```

### Helper Methods

#### `WithValue<U>(U newValue)`

Creates a new PipelineValue with a different value but preserves the validation state:

```csharp
var dateValue = new PipelineValue<DateTime>(DateTime.Now, isValid: true);
var stringValue = dateValue.WithValue("2024-12-10"); // Preserves IsValid and Errors
```

#### `WithValidation(bool isValid, PipelineError error = null)`

Creates a new PipelineValue with updated validation state:

```csharp
var value = new PipelineValue<int>(42, isValid: true);
var invalid = value.WithValidation(false, new PipelineError("Value too large", "Validation"));
```

#### `WithError(PipelineError error)` / `WithError(string message, string operation)`

Adds an error to the PipelineValue and sets `IsValid` to false:

```csharp
var value = new PipelineValue<string>("test");
var withError = value.WithError("Invalid format", "Extract");
```

## PipelineError

Represents an error that occurred during pipeline processing.

### Properties

- **`Message`** (`string`): The error message
- **`Operation`** (`string`): The operation or context where the error occurred
- **`Timestamp`** (`DateTime`): When the error occurred

### Creating Errors

```csharp
// Simple error
var error = new PipelineError("Extraction failed");

// Error with operation context
var error = new PipelineError("Value must be positive", "Validation");

// Errors automatically get timestamps
Console.WriteLine(error.Timestamp); // Current time when created
```

### String Representation

The `ToString()` method provides a formatted error message:

```csharp
var error = new PipelineError("Invalid date format", "ExtractDate");
Console.WriteLine(error.ToString());
// Output: [2024-12-10 14:30:00] ExtractDate: Invalid date format
```

## Validation State Propagation

Unlike traditional validation that throws exceptions, this library allows operations to continue even when validation fails. The failed state is tracked and can be checked at any point.

### How It Works

1. **Validation failures don't stop execution**: When a validation fails, `IsValid` is set to `false`, but the pipeline continues.

2. **Transformations respect validation state**: Transformations on invalid values typically become no-ops, preserving the invalid state.

3. **Errors accumulate**: Each validation failure adds an error to the `Errors` collection.

4. **Final check**: You can check `IsValid` at the end of the pipeline to determine if all validations passed.

### Example

```csharp
var result = "invalid-date"
    .ExtractDate()           // Fails: IsValid = false, error added
    .AddDays(1)              // No-op: preserves invalid state
    .Before(DateTime.Now)     // Still executes: adds another error
    .Format((v, isValid) => isValid ? "OK" : "Failed"); // "Failed"

// Check errors
if (!result.IsValid)
{
    Console.WriteLine($"Validation failed with {result.Errors.Count} errors:");
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"  [{error.Timestamp}] {error.Operation}: {error.Message}");
    }
}
```

### Benefits

- **Non-blocking**: Pipeline continues even after failures
- **Comprehensive error reporting**: All errors are collected, not just the first
- **Flexible error handling**: You can decide how to handle errors at the end
- **Debugging**: Full error history helps identify issues

## CrossValidationResult

Represents the result of cross-validation between unstructured and structured data using fuzzy matching.

### Properties

- **`FieldScores`** (`Dictionary<string, double>`): Similarity scores for each field (0.0 to 1.0)
- **`FieldMatches`** (`Dictionary<string, bool>`): Whether each field matched above threshold
- **`IsValid`** (`bool`): Overall validation result (true if all fields matched)
- **`MinSimilarity`** (`double`): Minimum similarity score across all fields
- **`MaxSimilarity`** (`double`): Maximum similarity score across all fields
- **`AverageSimilarity`** (`double`): Average similarity score across all fields
- **`BestMatchingField`** (`string`): Field name with highest similarity
- **`WorstMatchingField`** (`string`): Field name with lowest similarity

### Usage

```csharp
// Cross-validate unstructured text against structured data
var referenceData = new { Name = "John Smith", Address = "123 Main St" };
var fieldMappings = new Dictionary<string, string>
{
    { "Name", "name" },
    { "Address", "address" }
};

var result = "John Smith, 123 Main Street"
    .CrossValidate(referenceData, fieldMappings, new FuzzyMatchingConfig 
    { 
        SimilarityThreshold = 0.8 
    });

if (result.Value.IsValid)
{
    Console.WriteLine("All fields matched!");
}
else
{
    Console.WriteLine($"Best match: {result.Value.BestMatchingField}");
    Console.WriteLine($"Worst match: {result.Value.WorstMatchingField}");
    Console.WriteLine($"Average similarity: {result.Value.AverageSimilarity:F2}");
}
```

### Helper Methods

#### `GetFieldScore(string fieldName)`

Gets the similarity score for a specific field:

```csharp
double nameScore = result.Value.GetFieldScore("name");
```

#### `GetFieldMatch(string fieldName)`

Checks if a specific field matched above the threshold:

```csharp
bool nameMatched = result.Value.GetFieldMatch("name");
```

## Type Conversions

### Implicit Conversions

`PipelineValue<T>` supports implicit conversions for convenience:

```csharp
// Convert PipelineValue to its underlying value
PipelineValue<string> pv = new PipelineValue<string>("Hello");
string value = pv; // Implicitly unwraps to "Hello"

// Convert value to PipelineValue
PipelineValue<int> pv2 = 42; // Implicitly wraps 42 in a valid PipelineValue
```

### Explicit Conversions

For type safety, you can also use explicit conversions or access the `Value` property directly:

```csharp
var pv = new PipelineValue<int>(42);
int value = pv.Value; // Explicit access
```

## Best Practices

1. **Always check `IsValid`**: Before using the value in critical operations, check if it's valid.

2. **Review error collection**: Use the `Errors` collection to understand what went wrong.

3. **Leverage immutability**: Since operations return new instances, you can safely reuse original values.

4. **Use appropriate error messages**: When creating custom validations, provide meaningful error messages.

5. **Check timestamps**: Error timestamps can help debug timing issues in complex pipelines.

