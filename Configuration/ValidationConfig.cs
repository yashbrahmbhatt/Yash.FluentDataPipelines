using System;

namespace Yash.FluentDataPipelines.Configuration
{
    /// <summary>
    /// Configuration for validation operations.
    /// </summary>
    public class ValidationConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether string comparisons should be case-sensitive.
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// Gets or sets the StringComparison to use for string validations.
        /// </summary>
        public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

        /// <summary>
        /// Gets or sets the tolerance for numeric comparisons (e.g., for floating-point equality).
        /// </summary>
        public double? Tolerance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the lower bound is inclusive in range validations.
        /// </summary>
        public bool InclusiveLowerBound { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the upper bound is inclusive in range validations.
        /// </summary>
        public bool InclusiveUpperBound { get; set; } = true;

        /// <summary>
        /// Gets or sets a custom validation function.
        /// </summary>
        public Func<object, bool> CustomValidator { get; set; }

        /// <summary>
        /// Gets or sets the error message to use when validation fails.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates a default ValidationConfig.
        /// </summary>
        public static ValidationConfig Default => new ValidationConfig();
    }
}

