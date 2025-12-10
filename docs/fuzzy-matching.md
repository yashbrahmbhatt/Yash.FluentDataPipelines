# Fuzzy Matching

Fuzzy matching allows you to compare strings that are similar but not exactly identical. This is useful for handling typos, OCR errors, and variations in data formats. The library provides multiple fuzzy matching algorithms and normalization utilities.

## Fuzzy Matching Algorithms

The library supports three fuzzy matching algorithms:

### Levenshtein

Based on edit distance (number of character insertions, deletions, or substitutions needed).

- **Best for**: General typos and character-level differences
- **Performance**: O(n*m) where n and m are string lengths
- **Use case**: When you need to find strings that are close in terms of character edits

```csharp
var config = new FuzzyMatchingConfig
{
    Algorithm = FuzzyAlgorithm.Levenshtein,
    SimilarityThreshold = 0.8
};

var result = "John Smith"
    .FuzzyMatch("Jon Smith", config);
```

### Jaro

Measures similarity based on matching characters and transpositions.

- **Best for**: Strings with transpositions (swapped characters)
- **Performance**: O(n*m)
- **Use case**: When characters might be in different positions

```csharp
var config = new FuzzyMatchingConfig
{
    Algorithm = FuzzyAlgorithm.Jaro,
    SimilarityThreshold = 0.8
};

var result = "John Smith"
    .FuzzyMatch("Jhon Smith", config); // Handles transposition
```

### Jaro-Winkler (Default)

Enhancement of Jaro that gives higher weight to prefix matches.

- **Best for**: Names and strings where the beginning is more important
- **Performance**: O(n*m)
- **Use case**: When prefix similarity is important (e.g., names, addresses)

```csharp
var config = new FuzzyMatchingConfig
{
    Algorithm = FuzzyAlgorithm.JaroWinkler, // Default
    SimilarityThreshold = 0.8
};

var result = "John Smith"
    .FuzzyMatch("John Smyth", config); // Higher score due to prefix match
```

## Basic Fuzzy Matching

### FuzzyMatch

Compares a string against a reference string using fuzzy matching.

```csharp
// Basic usage
var result = "John Smith"
    .FuzzyMatch("Jon Smith");

// With configuration
var config = new FuzzyMatchingConfig
{
    SimilarityThreshold = 0.8,
    Algorithm = FuzzyAlgorithm.JaroWinkler
};

var result = "John Smith"
    .FuzzyMatch("Jon Smith", config);

if (result.IsValid)
{
    Console.WriteLine("Strings match!");
}
```

### FuzzyContains

Checks if a string contains a substring using fuzzy matching.

```csharp
var result = "Hello World"
    .FuzzyContains("Wrold"); // Handles typo in "World"

var config = new FuzzyMatchingConfig
{
    SimilarityThreshold = 0.7
};

var result = "The quick brown fox"
    .FuzzyContains("quik", config); // Handles typo
```

## Normalization

Normalization prepares strings for comparison by standardizing formats. This is especially useful for addresses, phone numbers, and names.

### NormalizeAddress

Normalizes address strings for comparison.

```csharp
// Basic normalization
var result = "123 Main St"
    .NormalizeAddress();

// With configuration
var config = new FuzzyMatchingConfig
{
    CaseSensitive = false
};

var result = "123 MAIN STREET"
    .NormalizeAddress(config);

// Then use in fuzzy matching
var normalized1 = "123 Main St".NormalizeAddress();
var normalized2 = "123 MAIN STREET".NormalizeAddress();
var match = normalized1.Value.FuzzyMatch(normalized2.Value);
```

**What it does:**
- Removes punctuation
- Expands abbreviations (St → Street, Ave → Avenue, etc.)
- Normalizes whitespace
- Optionally converts to lowercase
- Optionally removes common words (The, A, An, etc.)

### NormalizePhone

Normalizes phone number strings for comparison.

```csharp
// Basic normalization
var result = "(555) 123-4567"
    .NormalizePhone(); // "5551234567"

// With extensions
var result = "555-123-4567 x123"
    .NormalizePhone(); // "5551234567" (extension removed)

// Then compare
var phone1 = "(555) 123-4567".NormalizePhone();
var phone2 = "555-123-4567".NormalizePhone();
var match = phone1.Value.FuzzyMatch(phone2.Value); // Should match
```

**What it does:**
- Removes all non-digit characters
- Optionally removes extensions (x123, ext123, etc.)

### NormalizeName

Normalizes name strings for comparison.

```csharp
// Basic normalization
var result = "Mr. John Smith"
    .NormalizeName();

// With configuration
var config = new FuzzyMatchingConfig
{
    CaseSensitive = false
};

var result = "DR. JOHN SMITH"
    .NormalizeName(config);

// Then compare
var name1 = "Mr. John Smith".NormalizeName();
var name2 = "John Smith".NormalizeName();
var match = name1.Value.FuzzyMatch(name2.Value);
```

**What it does:**
- Optionally removes titles (Mr., Mrs., Dr., etc.)
- Normalizes whitespace
- Optionally converts to lowercase

## Typo Correction

### CorrectTypos

Finds the closest match in a dictionary of correct values and returns the corrected string.

```csharp
var correctValues = new[] { "John", "Jane", "Bob", "Alice" };

// Basic usage
var result = "Jon"
    .CorrectTypos(correctValues); // Returns "John"

// With configuration
var config = new FuzzyMatchingConfig
{
    SimilarityThreshold = 0.8,
    ReturnBestMatch = true // Return best match even if below threshold
};

var result = "Jhon"
    .CorrectTypos(correctValues, config);

if (result.IsValid)
{
    Console.WriteLine($"Corrected: {result.Value}"); // "John"
}
```

### Example: Correcting Product Names

```csharp
var products = new[] { "Widget", "Gadget", "Tool", "Device" };

var result = "Widgt" // Typo
    .CorrectTypos(products, new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.7
    });

Console.WriteLine(result.Value); // "Widget"
```

## Matching Against Multiple Values

### FuzzyMatchMany

Finds the best match from a collection of reference values.

```csharp
var referenceValues = new[] { "John Smith", "Jane Doe", "Bob Johnson" };

var result = "Jon Smith"
    .FuzzyMatchMany(referenceValues);

if (result.IsValid)
{
    var (bestMatch, similarity) = result.Value;
    Console.WriteLine($"Best match: {bestMatch} (similarity: {similarity:F2})");
    // Output: Best match: John Smith (similarity: 0.95)
}
```

### Batch Matching

### FuzzyMatchManyBatch

Matches each string in a collection against reference values.

```csharp
var sourceValues = new[] { "Jon Smith", "Jane Do", "Bob Jonson" };
var referenceValues = new[] { "John Smith", "Jane Doe", "Bob Johnson" };

var result = sourceValues
    .FuzzyMatchManyBatch(referenceValues, new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.8
    });

if (result.IsValid)
{
    foreach (var (original, bestMatch, similarity) in result.Value)
    {
        Console.WriteLine($"{original} -> {bestMatch} ({similarity:F2})");
    }
}
```

## Cross-Validation

Cross-validation compares unstructured data (like text extracted from documents) against structured reference data.

### CrossValidate

Validates unstructured text against structured reference data.

```csharp
// Define reference data
var referenceData = new
{
    Name = "John Smith",
    Address = "123 Main Street",
    Phone = "555-123-4567"
};

// Map reference fields to result fields
var fieldMappings = new Dictionary<string, string>
{
    { "Name", "name" },
    { "Address", "address" },
    { "Phone", "phone" }
};

// Perform cross-validation
var result = "John Smith, 123 Main St, 555-123-4567"
    .CrossValidate(referenceData, fieldMappings, new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.8,
        Algorithm = FuzzyAlgorithm.JaroWinkler
    });

if (result.IsValid && result.Value.IsValid)
{
    var validation = result.Value;
    Console.WriteLine($"All fields matched! Average similarity: {validation.AverageSimilarity:F2}");
    Console.WriteLine($"Best match: {validation.BestMatchingField}");
}
else
{
    var validation = result.Value;
    Console.WriteLine($"Validation failed. Worst match: {validation.WorstMatchingField}");
    Console.WriteLine($"Min similarity: {validation.MinSimilarity:F2}");
}
```

### Batch Cross-Validation

### CrossValidateMany

Validates multiple unstructured strings against structured reference data.

```csharp
var sourceTexts = new[]
{
    "John Smith, 123 Main St, 555-123-4567",
    "Jane Doe, 456 Oak Ave, 555-987-6543"
};

var referenceData = new
{
    Name = "John Smith",
    Address = "123 Main Street",
    Phone = "555-123-4567"
};

var fieldMappings = new Dictionary<string, string>
{
    { "Name", "name" },
    { "Address", "address" },
    { "Phone", "phone" }
};

var result = sourceTexts
    .CrossValidateMany(referenceData, fieldMappings, new FuzzyMatchingConfig
    {
        SimilarityThreshold = 0.8
    });

if (result.IsValid)
{
    foreach (var validation in result.Value)
    {
        Console.WriteLine($"Valid: {validation.IsValid}, Avg similarity: {validation.AverageSimilarity:F2}");
    }
}
```

## Configuration

### FuzzyMatchingConfig

Control fuzzy matching behavior:

```csharp
var config = new FuzzyMatchingConfig
{
    // Algorithm selection
    Algorithm = FuzzyAlgorithm.JaroWinkler, // Levenshtein, Jaro, or JaroWinkler
    
    // Similarity threshold (0.0 to 1.0)
    SimilarityThreshold = 0.8, // Higher = stricter matching
    
    // Case sensitivity
    CaseSensitive = false, // Default: false
    
    // Max edit distance (for Levenshtein only)
    MaxEditDistance = 3, // Optional: limit edit distance
    
    // Normalization options
    NormalizeAddress = true,  // Normalize addresses before comparison
    NormalizePhone = true,     // Normalize phone numbers before comparison
    NormalizeName = true,      // Normalize names before comparison
    
    // Custom similarity function
    CustomSimilarityFunction = (str1, str2) =>
    {
        // Your custom similarity calculation
        return 0.95; // Return 0.0 to 1.0
    },
    
    // Error message
    ErrorMessage = "Strings do not match",
    
    // Return best match even if below threshold
    ReturnBestMatch = false // Default: false
};
```

### Similarity Threshold

The similarity threshold determines how similar strings must be to be considered a match:

- **0.0**: Accepts any match (very lenient)
- **0.5**: Moderate matching
- **0.8**: Strict matching (default)
- **1.0**: Requires exact match (no fuzzy matching)

```csharp
// Lenient matching
var config = new FuzzyMatchingConfig
{
    SimilarityThreshold = 0.6
};
var result = "John Smith".FuzzyMatch("Jon Smyth", config); // More likely to match

// Strict matching
var config = new FuzzyMatchingConfig
{
    SimilarityThreshold = 0.9
};
var result = "John Smith".FuzzyMatch("Jon Smyth", config); // Less likely to match
```

## Best Practices

1. **Choose the right algorithm**: 
   - Use **Levenshtein** for general typos
   - Use **Jaro** for transpositions
   - Use **Jaro-Winkler** for names and addresses (default)

2. **Normalize before matching**: Normalize addresses, phone numbers, and names before comparison for better results.

3. **Set appropriate thresholds**: Adjust `SimilarityThreshold` based on your data quality and requirements.

4. **Use normalization flags**: Enable `NormalizeAddress`, `NormalizePhone`, or `NormalizeName` in config for automatic normalization.

5. **Consider case sensitivity**: Set `CaseSensitive = false` for most use cases.

6. **Use MaxEditDistance for Levenshtein**: Limit edit distance for better performance and accuracy.

7. **Test with your data**: Different algorithms work better for different types of data. Test to find the best fit.

8. **Combine with extraction**: Use fuzzy matching with extraction operations for handling messy data.

9. **Handle validation state**: Always check `IsValid` before using fuzzy matching results.

10. **Use custom similarity functions**: For domain-specific matching, provide a custom similarity function.

