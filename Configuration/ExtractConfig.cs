using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Defines the mode for fuzzy extraction.
    /// </summary>
    public enum FuzzyExtractionMode
    {
        /// <summary>
        /// No fuzzy matching (default, backward compatible).
        /// </summary>
        None,

        /// <summary>
        /// Try strict extraction first, use fuzzy if it fails.
        /// </summary>
        Fallback,

        /// <summary>
        /// Use fuzzy matching to find best candidate from regex matches.
        /// </summary>
        Primary
    }

    /// <summary>
    /// Configuration for extraction operations from strings.
    /// </summary>
    public class ExtractConfig
    {
        /// <summary>
        /// Default regex patterns for common extraction types.
        /// </summary>
        public static readonly Dictionary<string, string> DefaultRegexPatterns = new Dictionary<string, string>
        {
            { "Date", @"\d{1,4}[-\/]\d{1,2}[-\/]\d{1,4}|\d{4}-\d{2}-\d{2}|\d{2}/\d{2}/\d{4}|\d{2}-\d{2}-\d{4}|\d{4}/\d{2}/\d{2}" },
            { "Int", @"-?\d+" },
            { "Double", @"-?\d+\.?\d*" },
            { "Decimal", @"-?\d+\.?\d*" },
            { "Bool", @"(?i)true|false|yes|no|1|0" },
            { "Guid", @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}" }
        };
        /// <summary>
        /// Gets or sets the regular expression pattern to use for extraction.
        /// </summary>
        public string RegexPattern { get; set; }

        /// <summary>
        /// Gets or sets the regex group index to extract (0 = entire match, 1 = first group, etc.).
        /// </summary>
        public int GroupIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the regex options (e.g., IgnoreCase, Multiline).
        /// </summary>
        public RegexOptions RegexOptions { get; set; } = RegexOptions.None;

        /// <summary>
        /// Gets or sets the culture to use for parsing (for dates, numbers, etc.).
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Gets or sets the format string for DateTime parsing.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the format strings to try for DateTime parsing (in order).
        /// </summary>
        public string[] DateTimeFormats { get; set; }

        /// <summary>
        /// Gets or sets the DateTimeStyles to use for DateTime parsing.
        /// </summary>
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;

        /// <summary>
        /// Gets or sets the NumberStyles to use for numeric parsing.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;

        /// <summary>
        /// Gets or sets a value indicating whether to throw exceptions on parse failure.
        /// If false, returns invalid PipelineValue instead.
        /// </summary>
        public bool ThrowOnFailure { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to use default regex patterns when no custom pattern is provided.
        /// Default is true.
        /// </summary>
        public bool UseDefaultRegex { get; set; } = true;

        /// <summary>
        /// Gets or sets the fuzzy matching configuration for extraction operations.
        /// </summary>
        public FuzzyMatchingConfig FuzzyMatchingConfig { get; set; }

        /// <summary>
        /// Gets or sets the fuzzy extraction mode.
        /// Default is None (backward compatible).
        /// </summary>
        public FuzzyExtractionMode FuzzyExtractionMode { get; set; } = FuzzyExtractionMode.None;

        /// <summary>
        /// Creates a default ExtractConfig.
        /// </summary>
        public static ExtractConfig Default => new ExtractConfig();
    }
}

