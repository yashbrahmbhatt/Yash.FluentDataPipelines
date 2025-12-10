using System;
using System.Globalization;
using System.Linq;
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
        /// <param name="extractionType">Optional extraction type name for default regex patterns (e.g., "Date", "Int", "Double").</param>
        /// <returns>A PipelineValue containing the extracted value or an error state.</returns>
        public static PipelineValue<T> Extract<T>(
            this string source,
            ExtractConfig config,
            Func<string, ExtractConfig, T> converter,
            string extractionType = null)
        {
            if (string.IsNullOrEmpty(source))
            {
                return new PipelineValue<T>(default(T), false, new[] { new PipelineError("Source string is null or empty", "Extract") });
            }

            try
            {
                string extractedString = source;
                string regexPattern = config.RegexPattern;

                // Use default regex pattern if no custom pattern provided and UseDefaultRegex is enabled
                if (string.IsNullOrEmpty(regexPattern) && config.UseDefaultRegex && !string.IsNullOrEmpty(extractionType))
                {
                    if (ExtractConfig.DefaultRegexPatterns.TryGetValue(extractionType, out string defaultPattern))
                    {
                        regexPattern = defaultPattern;
                    }
                }

                // Apply regex if we have a pattern
                if (!string.IsNullOrEmpty(regexPattern))
                {
                    var regex = new Regex(regexPattern, config.RegexOptions);
                    var matches = regex.Matches(source);
                    
                    if (matches.Count == 0)
                    {
                        var error = new PipelineError($"Regex pattern '{regexPattern}' did not match", "Extract");
                        if (config.ThrowOnFailure)
                        {
                            throw new InvalidOperationException(error.Message);
                        }
                        return new PipelineValue<T>(default(T), false, new[] { error });
                    }

                    // Handle fuzzy extraction if enabled
                    if (config.FuzzyExtractionMode != FuzzyExtractionMode.None && config.FuzzyMatchingConfig != null && matches.Count > 1)
                    {
                        var matchValues = matches.Cast<Match>().Select(m => m.Groups[config.GroupIndex].Value).ToList();
                        
                        // Create validator based on extraction type
                        Func<string, bool> validator = null;
                        if (extractionType == "Date")
                        {
                            validator = (str) => FuzzyExtractionUtilities.TryParseDate(
                                str, 
                                config.DateTimeFormats ?? new[] { config.DateTimeFormat }.Where(f => !string.IsNullOrEmpty(f)).ToArray(),
                                config.Culture,
                                config.DateTimeStyles,
                                out _);
                        }
                        else if (extractionType == "Int")
                        {
                            validator = (str) => int.TryParse(str, config.NumberStyles, config.Culture, out _);
                        }
                        else if (extractionType == "Double" || extractionType == "Decimal")
                        {
                            validator = (str) => double.TryParse(str, config.NumberStyles, config.Culture, out _);
                        }

                        var bestMatch = FuzzyExtractionUtilities.FindBestMatch(matchValues, config.FuzzyMatchingConfig, validator);
                        if (bestMatch != null)
                        {
                            extractedString = bestMatch.Item1;
                        }
                        else if (config.FuzzyExtractionMode == FuzzyExtractionMode.Primary)
                        {
                            // Primary mode requires fuzzy match
                            var error = new PipelineError($"Fuzzy extraction failed: no match met similarity threshold {config.FuzzyMatchingConfig.SimilarityThreshold}", "Extract");
                            if (config.ThrowOnFailure)
                            {
                                throw new InvalidOperationException(error.Message);
                            }
                            return new PipelineValue<T>(default(T), false, new[] { error });
                        }
                        else
                        {
                            // Fallback mode: use first match if fuzzy fails
                            extractedString = matchValues[0];
                        }
                    }
                    else
                    {
                        // Standard extraction: use first match
                        var match = matches[0];
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
                }

                // Convert using the provided converter
                var value = converter(extractedString, config);
                return new PipelineValue<T>(value, true);
            }
            catch (Exception ex)
            {
                // If fuzzy fallback is enabled and this is a parse error, try fuzzy extraction
                if (config.FuzzyExtractionMode == FuzzyExtractionMode.Fallback && 
                    config.FuzzyMatchingConfig != null && 
                    !string.IsNullOrEmpty(extractionType) &&
                    (ex is FormatException || ex is ArgumentException))
                {
                    try
                    {
                        // Try fuzzy extraction as fallback
                        string regexPattern = config.RegexPattern;
                        if (string.IsNullOrEmpty(regexPattern) && config.UseDefaultRegex && ExtractConfig.DefaultRegexPatterns.TryGetValue(extractionType, out string defaultPattern))
                        {
                            regexPattern = defaultPattern;
                        }

                        if (!string.IsNullOrEmpty(regexPattern))
                        {
                            var matches = FuzzyExtractionUtilities.FindAllMatches(source, regexPattern, config.RegexOptions);
                            if (matches.Count > 0)
                            {
                                Func<string, bool> validator = null;
                                if (extractionType == "Date")
                                {
                                    validator = (str) => FuzzyExtractionUtilities.TryParseDate(
                                        str,
                                        config.DateTimeFormats ?? new[] { config.DateTimeFormat }.Where(f => !string.IsNullOrEmpty(f)).ToArray(),
                                        config.Culture,
                                        config.DateTimeStyles,
                                        out _);
                                }
                                else if (extractionType == "Int")
                                {
                                    validator = (str) => int.TryParse(str, config.NumberStyles, config.Culture, out _);
                                }
                                else if (extractionType == "Double" || extractionType == "Decimal")
                                {
                                    validator = (str) => double.TryParse(str, config.NumberStyles, config.Culture, out _);
                                }

                                var bestMatch = FuzzyExtractionUtilities.FindBestMatch(matches, config.FuzzyMatchingConfig, validator);
                                if (bestMatch != null)
                                {
                                    var value = converter(bestMatch.Item1, config);
                                    return new PipelineValue<T>(value, true);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Fall through to original error handling
                    }
                }

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
            }, "Date");
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
            }, "Int");
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
            }, "Double");
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
            }, "Decimal");
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
                // Handle common boolean representations
                str = str.Trim();
                if (str.Equals("1", StringComparison.OrdinalIgnoreCase) || 
                    str.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("y", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (str.Equals("0", StringComparison.OrdinalIgnoreCase) || 
                    str.Equals("no", StringComparison.OrdinalIgnoreCase) ||
                    str.Equals("n", StringComparison.OrdinalIgnoreCase))
                    return false;
                return bool.Parse(str);
            }, "Bool");
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
            }, "Guid");
        }
    }
}

