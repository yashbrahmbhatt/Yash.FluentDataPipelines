using System;
using System.Globalization;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Configuration for formatting operations.
    /// </summary>
    public class FormatConfig
    {
        /// <summary>
        /// Gets or sets the format string to use (e.g., "yyyy-MM-dd" for dates, "C" for currency).
        /// </summary>
        public string FormatString { get; set; }

        /// <summary>
        /// Gets or sets the culture to use for formatting.
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Gets or sets a custom formatter function that takes the value and returns a string.
        /// </summary>
        public Func<object, string> CustomFormatter { get; set; }

        /// <summary>
        /// Gets or sets a custom formatter function that takes the value and validation state, returns a string.
        /// </summary>
        public Func<object, bool, string> CustomFormatterWithValidation { get; set; }

        /// <summary>
        /// Gets or sets the string to use when the value is null.
        /// </summary>
        public string NullValueString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the string to use when the value is invalid.
        /// </summary>
        public string InvalidValueString { get; set; } = "Invalid";

        /// <summary>
        /// Creates a default FormatConfig.
        /// </summary>
        public static FormatConfig Default => new FormatConfig();
    }
}

