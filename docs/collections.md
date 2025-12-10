# Collection Operations

Collection operations allow you to work with collections (`IEnumerable<T>`) in your pipelines. These operations provide LINQ-like functionality while maintaining validation state propagation.

## Working with Collections

Collections in pipelines are wrapped in `PipelineValue<IEnumerable<T>>`. All collection operations preserve validation state.

### Creating Collection PipelineValues

```csharp
// From array
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

// From list
var names = new List<string> { "Alice", "Bob", "Charlie" };
var pipelineValue = new PipelineValue<IEnumerable<string>>(names);

// Implicit conversion
PipelineValue<IEnumerable<int>> pv = new[] { 1, 2, 3 };
```

## Filtering

### Where

Filters a collection using a predicate function.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var result = pipelineValue
    .Where(x => x > 2); // [3, 4, 5]

// Chain multiple filters
var result = pipelineValue
    .Where(x => x > 2)
    .Where(x => x < 5); // [3, 4]
```

### Complex Filtering

```csharp
var products = new[]
{
    new { Name = "Widget", Price = 99.99m, Category = "Electronics" },
    new { Name = "Gadget", Price = 49.99m, Category = "Electronics" },
    new { Name = "Tool", Price = 29.99m, Category = "Hardware" }
};

var pipelineValue = new PipelineValue<IEnumerable<dynamic>>(products);

var result = pipelineValue
    .Where(p => p.Category == "Electronics" && p.Price > 50m);
```

## Projection

### Select

Projects each element of a collection into a new form.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var result = pipelineValue
    .Select(x => x * 2); // [2, 4, 6, 8, 10]

// Transform to different type
var result = pipelineValue
    .Select(x => $"Number: {x}"); // ["Number: 1", "Number: 2", ...]
```

### Chaining Select and Where

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var result = pipelineValue
    .Where(x => x > 2)        // [3, 4, 5]
    .Select(x => x * 2)       // [6, 8, 10]
    .Where(x => x > 7);       // [8, 10]
```

## Element Access

### First

Gets the first element of a collection.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var result = pipelineValue
    .Where(x => x > 2)
    .First(); // 3

// Throws error if collection is empty
var empty = new int[0];
var emptyPipeline = new PipelineValue<IEnumerable<int>>(empty);
var result = emptyPipeline.First(); // Error: IsValid = false
```

### FirstOrDefault

Gets the first element of a collection, or a default value if empty.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

// With default value
var result = pipelineValue
    .Where(x => x > 10)
    .FirstOrDefault(0); // 0 (no match found)

// Without default (uses default(T))
var result = pipelineValue
    .Where(x => x > 10)
    .FirstOrDefault(); // 0 (default int)
```

### Last

Gets the last element of a collection.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var result = pipelineValue
    .Where(x => x < 4)
    .Last(); // 3
```

### LastOrDefault

Gets the last element of a collection, or a default value if empty.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<string>>(numbers.Select(x => x.ToString()));

var result = pipelineValue
    .Where(x => x.Length > 10)
    .LastOrDefault("No match"); // "No match"
```

## Custom Transformations

### Transform

Apply custom transformations to collections.

```csharp
var numbers = new[] { 1, 2, 3, 4, 5 };
var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);

var config = new TransformationConfig();

var result = pipelineValue
    .Transform(config, (collection, cfg) =>
    {
        // Calculate sum
        return collection.Sum();
    }); // 15

// Transform to different type
var result = pipelineValue
    .Transform(config, (collection, cfg) =>
    {
        // Get count
        return collection.Count();
    }); // 5
```

## Combining with Other Operations

### Extract and Filter

```csharp
var text = "1,2,3,4,5";
var numbers = text
    .Split(',')
    .Select(s => s.Trim().ExtractInt())
    .Where(pv => pv.IsValid)
    .Select(pv => pv.Value)
    .ToArray();

var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers);
var result = pipelineValue
    .Where(x => x > 2)
    .First(); // 3
```

### Transform and Validate

```csharp
var prices = new[] { "99.99", "49.99", "invalid", "29.99" };
var pipelineValues = prices
    .Select(p => p.ExtractDecimal())
    .ToArray();

var validPrices = pipelineValues
    .Where(pv => pv.IsValid)
    .Select(pv => pv.Value)
    .ToArray();

var pipelineValue = new PipelineValue<IEnumerable<decimal>>(validPrices);
var result = pipelineValue
    .Where(p => p > 50m)
    .First(); // 99.99
```

## Validation State

Collection operations preserve validation state. If the collection pipeline value is invalid, operations become no-ops:

```csharp
var invalidCollection = new PipelineValue<IEnumerable<int>>(
    null, 
    isValid: false, 
    errors: new[] { new PipelineError("Collection is null", "Create") }
);

var result = invalidCollection
    .Where(x => x > 2)  // No-op: preserves invalid state
    .First();           // No-op: preserves invalid state

// result.IsValid is still false
```

## Best Practices

1. **Filter early**: Apply filters as early as possible to reduce processing.

2. **Use FirstOrDefault for safety**: Prefer `FirstOrDefault` over `First` when the collection might be empty.

3. **Chain operations efficiently**: Combine `Where` and `Select` for efficient processing.

4. **Check validation state**: Verify `IsValid` before using collection results.

5. **Handle empty collections**: Use `FirstOrDefault` or `LastOrDefault` with appropriate default values.

6. **Transform collections carefully**: When transforming collections, ensure the transformation preserves the expected structure.

7. **Combine with extraction**: Use collection operations with extraction operations for complex data processing.

8. **Consider performance**: For large collections, be mindful of the performance implications of multiple operations.

