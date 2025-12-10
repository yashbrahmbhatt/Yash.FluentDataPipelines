using System;
using System.Globalization;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for formatting PipelineValue to strings.
    /// </summary>
    public static class FormatExtensions
    {
        /// <summary>
        /// Formats a PipelineValue using the provided configuration.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to format.</param>
        /// <param name="config">The format configuration.</param>
        /// <returns>A formatted string.</returns>
        public static string Format<T>(
            this PipelineValue<T> pipelineValue,
            FormatConfig config)
        {
            if (pipelineValue == null)
            {
                return "null";
            }

            // CustomFormatterWithValidation takes precedence over CustomFormatter
            if (config.CustomFormatterWithValidation != null)
            {
                return config.CustomFormatterWithValidation(pipelineValue.Value, pipelineValue.IsValid);
            }

            if (config.CustomFormatter != null)
            {
                return config.CustomFormatter(pipelineValue.Value);
            }

            if (pipelineValue.Value == null)
            {
                return config.NullValueString;
            }

            if (!pipelineValue.IsValid)
            {
                return config.InvalidValueString;
            }

            // Use format string if provided
            if (!string.IsNullOrEmpty(config.FormatString))
            {
                if (pipelineValue.Value is IFormattable formattable)
                {
                    return formattable.ToString(config.FormatString, config.Culture);
                }
            }

            // Default formatting
            return pipelineValue.Value.ToString();
        }

        /// <summary>
        /// Formats a PipelineValue using default formatting.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to format.</param>
        /// <returns>A formatted string.</returns>
        public static string Format<T>(this PipelineValue<T> pipelineValue)
        {
            return pipelineValue.Format(FormatConfig.Default);
        }

        /// <summary>
        /// Formats a PipelineValue using a custom format string.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to format.</param>
        /// <param name="formatString">The format string (e.g., "yyyy-MM-dd" for dates, "C" for currency).</param>
        /// <returns>A formatted string.</returns>
        public static string Format<T>(this PipelineValue<T> pipelineValue, string formatString)
        {
            var config = new FormatConfig { FormatString = formatString };
            return pipelineValue.Format(config);
        }

        /// <summary>
        /// Formats a PipelineValue using a custom formatter function.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to format.</param>
        /// <param name="formatter">Function that takes the value and returns a formatted string.</param>
        /// <returns>A formatted string.</returns>
        public static string Format<T>(this PipelineValue<T> pipelineValue, Func<T, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var config = new FormatConfig { CustomFormatter = v => formatter((T)v) };
            return pipelineValue.Format(config);
        }

        /// <summary>
        /// Formats a PipelineValue using a custom formatter function that receives both value and validation state.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="pipelineValue">The PipelineValue to format.</param>
        /// <param name="formatter">Function that takes the value and validation state, returns a formatted string.</param>
        /// <returns>A formatted string.</returns>
        public static string Format<T>(this PipelineValue<T> pipelineValue, Func<T, bool, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var config = new FormatConfig { CustomFormatterWithValidation = (v, isValid) => formatter((T)v, isValid) };
            return pipelineValue.Format(config);
        }

        /// <summary>
        /// Formats a DateTime PipelineValue as a date string.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a DateTime.</param>
        /// <param name="formatString">Optional format string (defaults to short date format).</param>
        /// <returns>A formatted date string.</returns>
        public static string FormatDate(
            this PipelineValue<DateTime> pipelineValue,
            string formatString = null)
        {
            if (formatString == null)
            {
                formatString = "d"; // Short date format
            }
            return pipelineValue.Format(formatString);
        }

        /// <summary>
        /// Formats a numeric PipelineValue as a number string.
        /// </summary>
        /// <typeparam name="T">The numeric type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a number.</param>
        /// <param name="formatString">Optional format string (defaults to "N2" for 2 decimal places).</param>
        /// <returns>A formatted number string.</returns>
        public static string FormatNumber<T>(
            this PipelineValue<T> pipelineValue,
            string formatString = "N2")
        {
            return pipelineValue.Format(formatString);
        }

        /// <summary>
        /// Formats a numeric PipelineValue as a currency string.
        /// </summary>
        /// <typeparam name="T">The numeric type.</typeparam>
        /// <param name="pipelineValue">The PipelineValue containing a number.</param>
        /// <param name="culture">Optional culture for currency formatting.</param>
        /// <returns>A formatted currency string.</returns>
        public static string FormatCurrency<T>(
            this PipelineValue<T> pipelineValue,
            CultureInfo culture = null)
        {
            var config = new FormatConfig
            {
                FormatString = "C",
                Culture = culture ?? CultureInfo.CurrentCulture
            };
            return pipelineValue.Format(config);
        }
    }
}

