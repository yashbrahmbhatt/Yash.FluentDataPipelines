using System;
using System.Globalization;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Configuration for transformation operations.
    /// </summary>
    public class TransformationConfig
    {
        /// <summary>
        /// Gets or sets the MidpointRounding mode for rounding operations.
        /// </summary>
        public MidpointRounding RoundingMode { get; set; } = MidpointRounding.ToEven;

        /// <summary>
        /// Gets or sets the culture to use for transformations (formatting, parsing, etc.).
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// Gets or sets a value indicating whether to trim whitespace in string transformations.
        /// </summary>
        public bool TrimWhitespace { get; set; } = true;

        /// <summary>
        /// Gets or sets the characters to trim (if null, trims all whitespace).
        /// </summary>
        public char[] TrimChars { get; set; }

        /// <summary>
        /// Gets or sets a custom transformation function.
        /// </summary>
        public Func<object, object> CustomTransform { get; set; }

        /// <summary>
        /// Creates a default TransformationConfig.
        /// </summary>
        public static TransformationConfig Default => new TransformationConfig();
    }
}

