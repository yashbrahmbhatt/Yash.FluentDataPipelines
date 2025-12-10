using System;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Algorithm types for fuzzy string matching.
    /// </summary>
    public enum FuzzyAlgorithm
    {
        /// <summary>
        /// Levenshtein distance-based similarity (edit distance).
        /// Good for general typos and character-level differences.
        /// </summary>
        Levenshtein,

        /// <summary>
        /// Jaro similarity algorithm.
        /// Good for strings with transpositions.
        /// </summary>
        Jaro,

        /// <summary>
        /// Jaro-Winkler similarity algorithm.
        /// Optimized for names, gives higher weight to prefix matches.
        /// </summary>
        JaroWinkler
    }

    /// <summary>
    /// Configuration for fuzzy matching operations.
    /// </summary>
    public class FuzzyMatchingConfig
    {
        /// <summary>
        /// Gets or sets the fuzzy matching algorithm to use.
        /// </summary>
        public FuzzyAlgorithm Algorithm { get; set; } = FuzzyAlgorithm.JaroWinkler;

        /// <summary>
        /// Gets or sets the similarity threshold (0.0 to 1.0) required for a match.
        /// Values closer to 1.0 require higher similarity.
        /// </summary>
        public double SimilarityThreshold { get; set; } = 0.8;

        /// <summary>
        /// Gets or sets a value indicating whether string comparisons should be case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum edit distance allowed (for Levenshtein algorithm).
        /// If null, no limit is applied.
        /// </summary>
        public int? MaxEditDistance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to normalize addresses before comparison.
        /// </summary>
        public bool NormalizeAddress { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to normalize phone numbers before comparison.
        /// </summary>
        public bool NormalizePhone { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to normalize names before comparison.
        /// </summary>
        public bool NormalizeName { get; set; } = false;

        /// <summary>
        /// Gets or sets a custom similarity function.
        /// If provided, this will be used instead of the selected algorithm.
        /// Function signature: (string1, string2) => similarity score (0.0 to 1.0)
        /// </summary>
        public Func<string, string, double> CustomSimilarityFunction { get; set; }

        /// <summary>
        /// Gets or sets the error message to use when fuzzy matching fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to return the best match even if below threshold (for batch operations).
        /// </summary>
        public bool ReturnBestMatch { get; set; } = false;

        /// <summary>
        /// Creates a default FuzzyMatchingConfig.
        /// </summary>
        public static FuzzyMatchingConfig Default => new FuzzyMatchingConfig();
    }
}

