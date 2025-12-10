using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for string extraction operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Extracts a value from a string using the provided configuration.
        /// </summary>
        /// <typeparam name="T">The target type to extract.</typeparam>
        /// <param name="source">The source string.</param>
        /// <param name="config">The extraction configuration.</param>
        /// <param name="converter">Function to convert the extracted string to type T.</param>
        /// <returns>A PipelineValue containing the extracted value or an error state.</returns>
        public static PipelineValue<T> Extract<T>(
            this string source,
            ExtractConfig config,
            Func<string, ExtractConfig, T> converter)
        {
            if (string.IsNullOrEmpty(source))
            {
                return new PipelineValue<T>(default(T), false, new[] { new PipelineError("Source string is null or empty", "Extract") });
            }

            try
            {
                string extractedString = source;

                // Apply regex if provided
                if (!string.IsNullOrEmpty(config.RegexPattern))
                {
                    var regex = new Regex(config.RegexPattern, config.RegexOptions);
                    var match = regex.Match(source);
                    if (!match.Success)
                    {
                        var error = new PipelineError($"Regex pattern '{config.RegexPattern}' did not match", "Extract");
                        if (config.ThrowOnFailure)
                        {
                            throw new InvalidOperationException(error.Message);
                        }
                        return new PipelineValue<T>(default(T), false, new[] { error });
                    }

                    if (config.GroupIndex < 0 || config.GroupIndex >= match.Groups.Count)
                    {
                        var error = new PipelineError($"Group index {config.GroupIndex} is out of range. Found {match.Groups.Count} groups.", "Extract");
                        if (config.ThrowOnFailure)
                        {
                            throw new InvalidOperationException(error.Message);
                        }
                        return new PipelineValue<T>(default(T), false, new[] { error });
                    }

                    extractedString = match.Groups[config.GroupIndex].Value;
                }

                // Convert using the provided converter
                var value = converter(extractedString, config);
                return new PipelineValue<T>(value, true);
            }
            catch (Exception ex)
            {
                var error = new PipelineError($"Extraction failed: {ex.Message}", "Extract");
                if (config.ThrowOnFailure)
                {
                    throw;
                }
                return new PipelineValue<T>(default(T), false, new[] { error });
            }
        }

        /// <summary>
        /// Extracts a DateTime from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted DateTime or an error state.</returns>
        public static PipelineValue<DateTime> ExtractDate(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                if (!string.IsNullOrEmpty(cfg.DateTimeFormat))
                {
                    return DateTime.ParseExact(str, cfg.DateTimeFormat, cfg.Culture, cfg.DateTimeStyles);
                }
                if (cfg.DateTimeFormats != null && cfg.DateTimeFormats.Length > 0)
                {
                    return DateTime.ParseExact(str, cfg.DateTimeFormats, cfg.Culture, cfg.DateTimeStyles);
                }
                return DateTime.Parse(str, cfg.Culture, cfg.DateTimeStyles);
            });
        }

        /// <summary>
        /// Extracts an integer from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted integer or an error state.</returns>
        public static PipelineValue<int> ExtractInt(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                return int.Parse(str, cfg.NumberStyles, cfg.Culture);
            });
        }

        /// <summary>
        /// Extracts a double from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted double or an error state.</returns>
        public static PipelineValue<double> ExtractDouble(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                return double.Parse(str, cfg.NumberStyles, cfg.Culture);
            });
        }

        /// <summary>
        /// Extracts a decimal from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted decimal or an error state.</returns>
        public static PipelineValue<decimal> ExtractDecimal(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                return decimal.Parse(str, cfg.NumberStyles, cfg.Culture);
            });
        }

        /// <summary>
        /// Extracts a boolean from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted boolean or an error state.</returns>
        public static PipelineValue<bool> ExtractBool(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                return bool.Parse(str);
            });
        }

        /// <summary>
        /// Extracts a Guid from a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional extraction configuration.</param>
        /// <returns>A PipelineValue containing the extracted Guid or an error state.</returns>
        public static PipelineValue<Guid> ExtractGuid(this string source, ExtractConfig config = null)
        {
            config = config ?? ExtractConfig.Default;
            return source.Extract(config, (str, cfg) =>
            {
                return Guid.Parse(str);
            });
        }
    }
}

