using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Configuration for extraction operations from strings.
    /// </summary>
    public class ExtractConfig
    {
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
        /// Creates a default ExtractConfig.
        /// </summary>
        public static ExtractConfig Default => new ExtractConfig();
    }
}

