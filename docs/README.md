# Documentation Index

Welcome to the FluentDataPipelines documentation! This folder contains detailed documentation for all features and operations in the library.

## Getting Started

If you're new to the library, start with:

1. **[Core Concepts](core-concepts.md)** - Understand the fundamental concepts like `PipelineValue<T>`, validation state propagation, and error handling
2. **[Extraction Operations](extraction.md)** - Learn how to extract typed values from strings
3. **[Quick Start Guide](../README.md#quick-start)** - See a simple example in the main README

## Feature Documentation

### Core Features

- **[Core Concepts](core-concepts.md)** - PipelineValue, PipelineError, CrossValidationResult, and validation state propagation
- **[Extraction Operations](extraction.md)** - Extract typed values from strings with regex, fuzzy matching, and custom parsing
- **[Validation Operations](validation.md)** - Validate dates, numbers, strings, and custom conditions
- **[Transformation Operations](transformation.md)** - Transform dates, numbers, strings, and collections
- **[Formatting Operations](formatting.md)** - Format values into strings with multiple options

### Advanced Features

- **[Collection Operations](collections.md)** - Work with collections using LINQ-like operations
- **[Fuzzy Matching](fuzzy-matching.md)** - Advanced fuzzy string matching, normalization, and cross-validation
- **[Configuration Reference](configuration.md)** - Complete reference for all configuration objects

## Documentation Structure

Each documentation file follows a consistent structure:

1. **Overview** - Introduction to the feature
2. **Basic Usage** - Simple examples to get started
3. **Advanced Usage** - More complex scenarios and configurations
4. **Examples** - Real-world use cases
5. **Best Practices** - Tips and recommendations

## Quick Reference

### Common Operations

**Extraction:**
```csharp
var date = "2024-12-10".ExtractDate();
var number = "42".ExtractInt();
var price = "99.99".ExtractDecimal();
```

**Validation:**
```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Before(DateTime.Now)
    .After(DateTime.Now.AddDays(-7));
```

**Transformation:**
```csharp
var result = "99.99"
    .ExtractDecimal()
    .Multiply(1.15m)
    .Round(2);
```

**Formatting:**
```csharp
var result = "2024-12-10"
    .ExtractDate()
    .Format("yyyy-MM-dd");
```

**Fuzzy Matching:**
```csharp
var result = "John Smith"
    .FuzzyMatch("Jon Smith", new FuzzyMatchingConfig 
    { 
        SimilarityThreshold = 0.8 
    });
```

## Examples by Use Case

### Data Cleaning
- [Extraction Operations](extraction.md) - Extract and parse data
- [Transformation Operations](transformation.md) - Clean and normalize data
- [Normalization](fuzzy-matching.md#normalization) - Normalize addresses, phone numbers, names

### Data Validation
- [Validation Operations](validation.md) - Validate data against rules
- [Fuzzy Matching](fuzzy-matching.md) - Validate with fuzzy matching
- [Cross-Validation](fuzzy-matching.md#cross-validation) - Validate unstructured against structured data

### Data Transformation
- [Transformation Operations](transformation.md) - Transform values
- [Collection Operations](collections.md) - Transform collections
- [Formatting Operations](formatting.md) - Format output

### Error Handling
- [Core Concepts](core-concepts.md#validation-state-propagation) - Understand validation state
- [PipelineError](core-concepts.md#pipelineerror) - Work with errors
- [Error Handling Examples](../README.md#examples) - See examples in main README

## Configuration

All operations can be configured using configuration objects:

- **[ExtractConfig](configuration.md#extractconfig)** - Configure extraction
- **[ValidationConfig](configuration.md#validationconfig)** - Configure validation
- **[TransformationConfig](configuration.md#transformationconfig)** - Configure transformations
- **[FormatConfig](configuration.md#formatconfig)** - Configure formatting
- **[FuzzyMatchingConfig](configuration.md#fuzzymatchingconfig)** - Configure fuzzy matching

## Need Help?

- Check the [main README](../README.md) for installation and quick start
- Review [Best Practices](../README.md#best-practices) in the main README
- See [Examples](../README.md#examples) for common use cases
- Refer to [Configuration Reference](configuration.md) for all configuration options

## Contributing

Found an issue with the documentation? Please open an issue or submit a pull request!

