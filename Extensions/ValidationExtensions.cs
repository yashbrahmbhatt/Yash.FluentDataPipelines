using System;
using System.Collections.Generic;
using System.Linq;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for validation operations on PipelineValue.
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates a PipelineValue using the provided configuration.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to validate.</param>
        /// <param name="config">The validation configuration.</param>
        /// <param name="validator">Function that performs the validation.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<T> Validate<T>(
            this PipelineValue<T> pipelineValue,
            ValidationConfig config,
            Func<T, ValidationConfig, bool> validator)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<T>(default(T), false, new[] { new PipelineError("PipelineValue is null", "Validate") });
            }

            try
            {
                bool isValid = validator(pipelineValue.Value, config);
                string errorMessage = config.ErrorMessage ?? "Validation failed";
                var error = isValid ? null : new PipelineError(errorMessage, "Validate");
                return pipelineValue.WithValidation(isValid, error);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Validation error: {ex.Message}", "Validate");
            }
        }

        /// <summary>
        /// Validates that a DateTime is before the specified date.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="date">The date to compare against.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<DateTime> Before(
            this PipelineValue<DateTime> pipelineValue,
            DateTime date,
            ValidationConfig config = null)
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) => value < date);
        }

        /// <summary>
        /// Validates that a DateTime is after the specified date.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="date">The date to compare against.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<DateTime> After(
            this PipelineValue<DateTime> pipelineValue,
            DateTime date,
            ValidationConfig config = null)
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) => value > date);
        }

        /// <summary>
        /// Validates that a value is between two bounds (inclusive by default).
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable).</typeparam>
        /// <param name="pipelineValue">The PipelineValue to validate.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<T> Between<T>(
            this PipelineValue<T> pipelineValue,
            T lowerBound,
            T upperBound,
            ValidationConfig config = null) where T : IComparable<T>
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) =>
            {
                int lowerCompare = value.CompareTo(lowerBound);
                int upperCompare = value.CompareTo(upperBound);

                bool lowerValid = cfg.InclusiveLowerBound ? lowerCompare >= 0 : lowerCompare > 0;
                bool upperValid = cfg.InclusiveUpperBound ? upperCompare <= 0 : upperCompare < 0;

                return lowerValid && upperValid;
            });
        }

        /// <summary>
        /// Validates that a value is greater than the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable).</typeparam>
        /// <param name="pipelineValue">The PipelineValue to validate.</param>
        /// <param name="other">The value to compare against.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<T> GreaterThan<T>(
            this PipelineValue<T> pipelineValue,
            T other,
            ValidationConfig config = null) where T : IComparable<T>
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) => value.CompareTo(other) > 0);
        }

        /// <summary>
        /// Validates that a value is less than the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable).</typeparam>
        /// <param name="pipelineValue">The PipelineValue to validate.</param>
        /// <param name="other">The value to compare against.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<T> LessThan<T>(
            this PipelineValue<T> pipelineValue,
            T other,
            ValidationConfig config = null) where T : IComparable<T>
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) => value.CompareTo(other) < 0);
        }

        /// <summary>
        /// Validates that a string contains the specified substring.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="substring">The substring to search for.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> Contains(
            this PipelineValue<string> pipelineValue,
            string substring,
            ValidationConfig config = null)
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) =>
            {
                if (value == null || substring == null)
                {
                    return false;
                }
                return value.IndexOf(substring, cfg.StringComparison) >= 0;
            });
        }

        /// <summary>
        /// Validates that a string matches the specified regular expression pattern.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="pattern">The regular expression pattern.</param>
        /// <param name="config">Optional validation configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> Matches(
            this PipelineValue<string> pipelineValue,
            string pattern,
            ValidationConfig config = null)
        {
            config = config ?? ValidationConfig.Default;
            return pipelineValue.Validate(config, (value, cfg) =>
            {
                if (value == null || pattern == null)
                {
                    return false;
                }
                var regexOptions = cfg.CaseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                return System.Text.RegularExpressions.Regex.IsMatch(value, pattern, regexOptions);
            });
        }

        /// <summary>
        /// Validates that a numeric value is approximately equal to another value within tolerance.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a double.</param>
        /// <param name="other">The value to compare against.</param>
        /// <param name="config">Optional validation configuration (tolerance should be set in config).</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<double> ApproximatelyEqual(
            this PipelineValue<double> pipelineValue,
            double other,
            ValidationConfig config = null)
        {
            config = config ?? ValidationConfig.Default;
            double tolerance = config.Tolerance ?? double.Epsilon;
            return pipelineValue.Validate(config, (value, cfg) => Math.Abs(value - other) <= tolerance);
        }
    }
}

