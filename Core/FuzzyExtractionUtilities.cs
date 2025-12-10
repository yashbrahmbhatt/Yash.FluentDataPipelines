using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yash.FluentDataPipelines.Configuration;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Utilities for fuzzy extraction operations.
    /// </summary>
    public static class FuzzyExtractionUtilities
    {
        /// <summary>
        /// Finds the best matching candidate from regex matches using fuzzy matching.
        /// </summary>
        /// <param name="matches">Collection of regex match values.</param>
        /// <param name="config">Fuzzy matching configuration.</param>
        /// <param name="validator">Optional function to validate if a match can be parsed to the target type.</param>
        /// <returns>Tuple containing the best match and its similarity score, or null if no good match found.</returns>
        public static Tuple<string, double> FindBestMatch(
            IEnumerable<string> matches,
            FuzzyMatchingConfig config,
            Func<string, bool> validator = null)
        {
            if (matches == null || config == null)
                return null;

            string bestMatch = null;
            double bestSimilarity = 0.0;

            foreach (string match in matches)
            {
                if (string.IsNullOrEmpty(match))
                    continue;

                // If validator is provided, only consider matches that pass validation
                if (validator != null && !validator(match))
                    continue;

                // For extraction, we want to find matches that are "close" to valid patterns
                // Since we're extracting from regex, we'll score based on how "clean" the match is
                // For now, we'll use a simple heuristic: prefer longer, more complete matches
                // In the future, this could be enhanced with pattern-specific scoring

                // Calculate a base similarity score
                // For extraction, we prefer matches that are more likely to parse correctly
                double similarity = CalculateExtractionSimilarity(match, config);

                if (similarity > bestSimilarity)
                {
                    bestSimilarity = similarity;
                    bestMatch = match;
                }
            }

            // Return best match if it meets the threshold
            if (bestMatch != null && bestSimilarity >= config.SimilarityThreshold)
            {
                return Tuple.Create(bestMatch, bestSimilarity);
            }

            return null;
        }

        /// <summary>
        /// Calculates a similarity score for extraction purposes.
        /// This is different from validation similarity - we're scoring how "extractable" a string is.
        /// </summary>
        private static double CalculateExtractionSimilarity(string match, FuzzyMatchingConfig config)
        {
            if (string.IsNullOrEmpty(match))
                return 0.0;

            // Base score: length and completeness
            // Longer, more complete matches are generally better
            double lengthScore = Math.Min(1.0, match.Length / 20.0); // Normalize to 20 chars

            // Character quality: prefer alphanumeric and common separators
            int validChars = 0;
            foreach (char c in match)
            {
                if (char.IsLetterOrDigit(c) || c == '-' || c == '/' || c == '.' || c == ':' || c == ' ')
                    validChars++;
            }
            double qualityScore = match.Length > 0 ? (double)validChars / match.Length : 0.0;

            // Combine scores
            return (lengthScore * 0.4 + qualityScore * 0.6);
        }

        /// <summary>
        /// Finds all regex matches in a string using the provided pattern.
        /// </summary>
        public static List<string> FindAllMatches(string source, string pattern, RegexOptions options = RegexOptions.None)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
                return new List<string>();

            var regex = new Regex(pattern, options);
            var matches = regex.Matches(source);
            return matches.Cast<Match>().Select(m => m.Value).ToList();
        }

        /// <summary>
        /// Attempts to parse a date string using multiple formats.
        /// </summary>
        public static bool TryParseDate(string value, string[] formats, System.Globalization.CultureInfo culture, System.Globalization.DateTimeStyles styles, out DateTime result)
        {
            result = default(DateTime);

            if (string.IsNullOrEmpty(value))
                return false;

            // Try exact formats first
            if (formats != null && formats.Length > 0)
            {
                foreach (string format in formats)
                {
                    if (DateTime.TryParseExact(value, format, culture, styles, out result))
                        return true;
                }
            }

            // Try general parse
            return DateTime.TryParse(value, culture, styles, out result);
        }

        /// <summary>
        /// Attempts to parse a numeric string.
        /// </summary>
        public static bool TryParseNumber<T>(string value, System.Globalization.NumberStyles styles, System.Globalization.CultureInfo culture, out T result) where T : struct
        {
            result = default(T);

            if (string.IsNullOrEmpty(value))
                return false;

            try
            {
                if (typeof(T) == typeof(int))
                {
                    if (int.TryParse(value, styles, culture, out int intResult))
                    {
                        result = (T)(object)intResult;
                        return true;
                    }
                }
                else if (typeof(T) == typeof(double))
                {
                    if (double.TryParse(value, styles, culture, out double doubleResult))
                    {
                        result = (T)(object)doubleResult;
                        return true;
                    }
                }
                else if (typeof(T) == typeof(decimal))
                {
                    if (decimal.TryParse(value, styles, culture, out decimal decimalResult))
                    {
                        result = (T)(object)decimalResult;
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}

