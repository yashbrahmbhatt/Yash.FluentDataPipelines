using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Extensions
{
    /// <summary>
    /// Extension methods for fuzzy matching operations on strings and PipelineValue.
    /// </summary>
    public static class FuzzyMatchingExtensions
    {
        #region String Extensions (direct string operations)

        /// <summary>
        /// Performs fuzzy matching validation against a reference string.
        /// </summary>
        /// <param name="source">The source string to validate.</param>
        /// <param name="reference">The reference string to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> FuzzyMatch(
            this string source,
            string reference,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).FuzzyMatch(reference, config);
        }

        /// <summary>
        /// Performs fuzzy substring matching validation.
        /// </summary>
        /// <param name="source">The source string to validate.</param>
        /// <param name="substring">The substring to search for using fuzzy matching.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> FuzzyContains(
            this string source,
            string substring,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).FuzzyContains(substring, config);
        }

        /// <summary>
        /// Corrects typos by finding the closest match in a dictionary of correct values.
        /// </summary>
        /// <param name="source">The source string to correct.</param>
        /// <param name="correctValues">Dictionary or collection of correct values to match against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the corrected string (or original if no good match found).</returns>
        public static PipelineValue<string> CorrectTypos(
            this string source,
            IEnumerable<string> correctValues,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).CorrectTypos(correctValues, config);
        }

        /// <summary>
        /// Performs cross-validation between unstructured data and structured reference data.
        /// </summary>
        /// <param name="source">The source string containing unstructured data.</param>
        /// <param name="referenceData">The structured reference data object.</param>
        /// <param name="fieldMappings">Dictionary mapping field names in referenceData to field names for comparison.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a CrossValidationResult.</returns>
        public static PipelineValue<CrossValidationResult> CrossValidate(
            this string source,
            object referenceData,
            Dictionary<string, string> fieldMappings,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).CrossValidate(referenceData, fieldMappings, config);
        }

        /// <summary>
        /// Performs fuzzy matching against multiple reference values and returns the best match.
        /// </summary>
        /// <param name="source">The source string to match.</param>
        /// <param name="referenceValues">Collection of reference strings to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a tuple of (bestMatch, similarityScore).</returns>
        public static PipelineValue<Tuple<string, double>> FuzzyMatchMany(
            this string source,
            IEnumerable<string> referenceValues,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).FuzzyMatchMany(referenceValues, config);
        }

        /// <summary>
        /// Normalizes an address string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized address string.</returns>
        public static PipelineValue<string> NormalizeAddress(
            this string source,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).NormalizeAddress(config);
        }

        /// <summary>
        /// Normalizes a phone number string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized phone number string.</returns>
        public static PipelineValue<string> NormalizePhone(
            this string source,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).NormalizePhone(config);
        }

        /// <summary>
        /// Normalizes a name string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized name string.</returns>
        public static PipelineValue<string> NormalizeName(
            this string source,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<string>(source).NormalizeName(config);
        }

        /// <summary>
        /// Performs batch fuzzy matching: compares each string in a collection against reference values.
        /// </summary>
        /// <param name="source">The collection of strings to match.</param>
        /// <param name="referenceValues">Collection of reference strings to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a collection of tuples (originalValue, bestMatch, similarityScore).</returns>
        public static PipelineValue<IEnumerable<Tuple<string, string, double>>> FuzzyMatchManyBatch(
            this IEnumerable<string> source,
            IEnumerable<string> referenceValues,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<IEnumerable<string>>(source).FuzzyMatchManyBatch(referenceValues, config);
        }

        /// <summary>
        /// Performs batch cross-validation: compares each string in a collection against structured reference data.
        /// </summary>
        /// <param name="source">The collection of strings to validate.</param>
        /// <param name="referenceData">The structured reference data object.</param>
        /// <param name="fieldMappings">Dictionary mapping field names in referenceData to field names for comparison.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a collection of CrossValidationResults.</returns>
        public static PipelineValue<IEnumerable<CrossValidationResult>> CrossValidateMany(
            this IEnumerable<string> source,
            object referenceData,
            Dictionary<string, string> fieldMappings,
            FuzzyMatchingConfig config = null)
        {
            return new PipelineValue<IEnumerable<string>>(source).CrossValidateMany(referenceData, fieldMappings, config);
        }

        #endregion

        #region PipelineValue Extensions
        /// <summary>
        /// Normalizes an address string using the provided configuration.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized address string.</returns>
        public static PipelineValue<string> NormalizeAddress(
            this PipelineValue<string> pipelineValue,
            FuzzyMatchingConfig config = null)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "NormalizeAddress") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string normalized = NormalizationUtilities.NormalizeAddress(
                    pipelineValue.Value,
                    toLowercase: config == null || config.CaseSensitive == false,
                    removeCommonWords: false);
                return pipelineValue.WithValue(normalized);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Normalization error: {ex.Message}", "NormalizeAddress");
            }
        }

        /// <summary>
        /// Normalizes a phone number string using the provided configuration.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized phone number string.</returns>
        public static PipelineValue<string> NormalizePhone(
            this PipelineValue<string> pipelineValue,
            FuzzyMatchingConfig config = null)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "NormalizePhone") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string normalized = NormalizationUtilities.NormalizePhone(pipelineValue.Value, removeExtensions: true);
                return pipelineValue.WithValue(normalized);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Normalization error: {ex.Message}", "NormalizePhone");
            }
        }

        /// <summary>
        /// Normalizes a name string using the provided configuration.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the normalized name string.</returns>
        public static PipelineValue<string> NormalizeName(
            this PipelineValue<string> pipelineValue,
            FuzzyMatchingConfig config = null)
        {
            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "NormalizeName") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string normalized = NormalizationUtilities.NormalizeName(
                    pipelineValue.Value,
                    toLowercase: config == null || config.CaseSensitive == false,
                    removeTitles: false);
                return pipelineValue.WithValue(normalized);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Normalization error: {ex.Message}", "NormalizeName");
            }
        }

        /// <summary>
        /// Performs fuzzy matching validation against a reference string.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string to validate.</param>
        /// <param name="reference">The reference string to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> FuzzyMatch(
            this PipelineValue<string> pipelineValue,
            string reference,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "FuzzyMatch") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string value = pipelineValue.Value;
                string refValue = reference;

                // Apply normalization if configured
                if (config.NormalizeAddress)
                {
                    value = NormalizationUtilities.NormalizeAddress(value, config.CaseSensitive == false);
                    refValue = NormalizationUtilities.NormalizeAddress(refValue, config.CaseSensitive == false);
                }
                else if (config.NormalizePhone)
                {
                    value = NormalizationUtilities.NormalizePhone(value);
                    refValue = NormalizationUtilities.NormalizePhone(refValue);
                }
                else if (config.NormalizeName)
                {
                    value = NormalizationUtilities.NormalizeName(value, config.CaseSensitive == false);
                    refValue = NormalizationUtilities.NormalizeName(refValue, config.CaseSensitive == false);
                }

                // Apply case sensitivity
                if (!config.CaseSensitive)
                {
                    value = value?.ToLowerInvariant();
                    refValue = refValue?.ToLowerInvariant();
                }

                // Check max edit distance for Levenshtein
                if (config.Algorithm == FuzzyAlgorithm.Levenshtein && config.MaxEditDistance.HasValue)
                {
                    int distance = FuzzyMatchingAlgorithms.LevenshteinDistance(value ?? "", refValue ?? "");
                    if (distance > config.MaxEditDistance.Value)
                    {
                        string errorMessage = config.ErrorMessage ?? $"Fuzzy match failed: edit distance {distance} exceeds maximum {config.MaxEditDistance.Value}";
                        return pipelineValue.WithValidation(false, new PipelineError(errorMessage, "FuzzyMatch"));
                    }
                }

                // Calculate similarity
                double similarity = config.CustomSimilarityFunction != null
                    ? config.CustomSimilarityFunction(value ?? "", refValue ?? "")
                    : FuzzyMatchingAlgorithms.CalculateSimilarity(value ?? "", refValue ?? "", config.Algorithm);

                bool isValid = similarity >= config.SimilarityThreshold;
                string errorMessage2 = config.ErrorMessage ?? $"Fuzzy match failed: similarity {similarity:F2} below threshold {config.SimilarityThreshold:F2}";
                var error = isValid ? null : new PipelineError(errorMessage2, "FuzzyMatch");

                return pipelineValue.WithValidation(isValid, error);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Fuzzy matching error: {ex.Message}", "FuzzyMatch");
            }
        }

        /// <summary>
        /// Performs fuzzy substring matching validation.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string to validate.</param>
        /// <param name="substring">The substring to search for using fuzzy matching.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with updated validation state.</returns>
        public static PipelineValue<string> FuzzyContains(
            this PipelineValue<string> pipelineValue,
            string substring,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "FuzzyContains") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string value = pipelineValue.Value;
                string subValue = substring;

                if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(subValue))
                {
                    return pipelineValue.WithValidation(false, new PipelineError("Fuzzy contains failed: input or substring is null or empty", "FuzzyContains"));
                }

                // Apply normalization if configured
                if (config.NormalizeAddress)
                {
                    value = NormalizationUtilities.NormalizeAddress(value, config.CaseSensitive == false);
                    subValue = NormalizationUtilities.NormalizeAddress(subValue, config.CaseSensitive == false);
                }
                else if (config.NormalizePhone)
                {
                    value = NormalizationUtilities.NormalizePhone(value);
                    subValue = NormalizationUtilities.NormalizePhone(subValue);
                }
                else if (config.NormalizeName)
                {
                    value = NormalizationUtilities.NormalizeName(value, config.CaseSensitive == false);
                    subValue = NormalizationUtilities.NormalizeName(subValue, config.CaseSensitive == false);
                }

                // Apply case sensitivity
                if (!config.CaseSensitive)
                {
                    value = value.ToLowerInvariant();
                    subValue = subValue.ToLowerInvariant();
                }

                // Try sliding window approach for fuzzy substring matching
                int subLength = subValue.Length;
                double bestSimilarity = 0.0;

                for (int i = 0; i <= value.Length - subLength; i++)
                {
                    string window = value.Substring(i, Math.Min(subLength, value.Length - i));
                    double similarity = config.CustomSimilarityFunction != null
                        ? config.CustomSimilarityFunction(window, subValue)
                        : FuzzyMatchingAlgorithms.CalculateSimilarity(window, subValue, config.Algorithm);

                    if (similarity > bestSimilarity)
                    {
                        bestSimilarity = similarity;
                    }

                    // Early exit if we found a perfect match
                    if (bestSimilarity >= 1.0)
                        break;
                }

                // Also check if substring is longer than value (partial match)
                if (subLength > value.Length)
                {
                    double similarity = config.CustomSimilarityFunction != null
                        ? config.CustomSimilarityFunction(value, subValue)
                        : FuzzyMatchingAlgorithms.CalculateSimilarity(value, subValue, config.Algorithm);
                    if (similarity > bestSimilarity)
                    {
                        bestSimilarity = similarity;
                    }
                }

                bool isValid = bestSimilarity >= config.SimilarityThreshold;
                string errorMessage = config.ErrorMessage ?? $"Fuzzy contains failed: best similarity {bestSimilarity:F2} below threshold {config.SimilarityThreshold:F2}";
                var error = isValid ? null : new PipelineError(errorMessage, "FuzzyContains");

                return pipelineValue.WithValidation(isValid, error);
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Fuzzy contains error: {ex.Message}", "FuzzyContains");
            }
        }

        /// <summary>
        /// Corrects typos by finding the closest match in a dictionary of correct values.
        /// This is a transformation operation that returns the corrected value.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string to correct.</param>
        /// <param name="correctValues">Dictionary or collection of correct values to match against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue with the corrected string (or original if no good match found).</returns>
        public static PipelineValue<string> CorrectTypos(
            this PipelineValue<string> pipelineValue,
            IEnumerable<string> correctValues,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<string>(default(string), false, new[] { new PipelineError("PipelineValue is null", "CorrectTypos") });
            }

            if (!pipelineValue.IsValid)
            {
                return pipelineValue;
            }

            try
            {
                string value = pipelineValue.Value;
                if (string.IsNullOrEmpty(value) || correctValues == null)
                {
                    return pipelineValue;
                }

                string normalizedValue = value;
                if (config.NormalizeAddress)
                {
                    normalizedValue = NormalizationUtilities.NormalizeAddress(normalizedValue, config.CaseSensitive == false);
                }
                else if (config.NormalizePhone)
                {
                    normalizedValue = NormalizationUtilities.NormalizePhone(normalizedValue);
                }
                else if (config.NormalizeName)
                {
                    normalizedValue = NormalizationUtilities.NormalizeName(normalizedValue, config.CaseSensitive == false);
                }

                if (!config.CaseSensitive)
                {
                    normalizedValue = normalizedValue.ToLowerInvariant();
                }

                string bestMatch = null;
                double bestSimilarity = 0.0;

                foreach (string correctValue in correctValues)
                {
                    if (string.IsNullOrEmpty(correctValue))
                        continue;

                    string normalizedCorrect = correctValue;
                    if (config.NormalizeAddress)
                    {
                        normalizedCorrect = NormalizationUtilities.NormalizeAddress(normalizedCorrect, config.CaseSensitive == false);
                    }
                    else if (config.NormalizePhone)
                    {
                        normalizedCorrect = NormalizationUtilities.NormalizePhone(normalizedCorrect);
                    }
                    else if (config.NormalizeName)
                    {
                        normalizedCorrect = NormalizationUtilities.NormalizeName(normalizedCorrect, config.CaseSensitive == false);
                    }

                    if (!config.CaseSensitive)
                    {
                        normalizedCorrect = normalizedCorrect.ToLowerInvariant();
                    }

                    double similarity = config.CustomSimilarityFunction != null
                        ? config.CustomSimilarityFunction(normalizedValue, normalizedCorrect)
                        : FuzzyMatchingAlgorithms.CalculateSimilarity(normalizedValue, normalizedCorrect, config.Algorithm);

                    if (similarity > bestSimilarity)
                    {
                        bestSimilarity = similarity;
                        bestMatch = correctValue; // Keep original case/format of correct value
                    }
                }

                // Return corrected value if similarity is above threshold, otherwise return original
                if (bestMatch != null && bestSimilarity >= config.SimilarityThreshold)
                {
                    return pipelineValue.WithValue(bestMatch);
                }

                // If ReturnBestMatch is true, return best match even if below threshold
                if (config.ReturnBestMatch && bestMatch != null)
                {
                    return pipelineValue.WithValue(bestMatch);
                }

                return pipelineValue;
            }
            catch (Exception ex)
            {
                return pipelineValue.WithError($"Typo correction error: {ex.Message}", "CorrectTypos");
            }
        }

        /// <summary>
        /// Performs cross-validation between unstructured data and structured reference data.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing unstructured string data.</param>
        /// <param name="referenceData">The structured reference data object.</param>
        /// <param name="fieldMappings">Dictionary mapping field names in referenceData to field names for comparison.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a CrossValidationResult.</returns>
        public static PipelineValue<CrossValidationResult> CrossValidate(
            this PipelineValue<string> pipelineValue,
            object referenceData,
            Dictionary<string, string> fieldMappings,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<CrossValidationResult>(null, false, new[] { new PipelineError("PipelineValue is null", "CrossValidate") });
            }

            if (!pipelineValue.IsValid)
            {
                return new PipelineValue<CrossValidationResult>(null, false, pipelineValue.Errors);
            }

            try
            {
                if (referenceData == null)
                {
                    return new PipelineValue<CrossValidationResult>(null, false, new[] { new PipelineError("Reference data is null", "CrossValidate") });
                }

                if (fieldMappings == null || fieldMappings.Count == 0)
                {
                    return new PipelineValue<CrossValidationResult>(null, false, new[] { new PipelineError("Field mappings are null or empty", "CrossValidate") });
                }

                string value = pipelineValue.Value;
                var fieldScores = new Dictionary<string, double>();

                // Use reflection to get property values from reference data
                Type refType = referenceData.GetType();
                PropertyInfo[] properties = refType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var mapping in fieldMappings)
                {
                    string sourceField = mapping.Key; // Field in referenceData
                    string targetField = mapping.Value; // Field name for result

                    // Find the property in referenceData
                    PropertyInfo prop = properties.FirstOrDefault(p => p.Name.Equals(sourceField, StringComparison.OrdinalIgnoreCase));
                    if (prop == null)
                    {
                        fieldScores[targetField] = 0.0;
                        continue;
                    }

                    object refValueObj = prop.GetValue(referenceData);
                    string refValue = refValueObj?.ToString() ?? "";

                    // Prepare strings for comparison
                    string normalizedValue = value;
                    string normalizedRef = refValue;

                    // Apply normalization if configured
                    if (config.NormalizeAddress)
                    {
                        normalizedValue = NormalizationUtilities.NormalizeAddress(normalizedValue, config.CaseSensitive == false);
                        normalizedRef = NormalizationUtilities.NormalizeAddress(normalizedRef, config.CaseSensitive == false);
                    }
                    else if (config.NormalizePhone)
                    {
                        normalizedValue = NormalizationUtilities.NormalizePhone(normalizedValue);
                        normalizedRef = NormalizationUtilities.NormalizePhone(normalizedRef);
                    }
                    else if (config.NormalizeName)
                    {
                        normalizedValue = NormalizationUtilities.NormalizeName(normalizedValue, config.CaseSensitive == false);
                        normalizedRef = NormalizationUtilities.NormalizeName(normalizedRef, config.CaseSensitive == false);
                    }

                    if (!config.CaseSensitive)
                    {
                        normalizedValue = normalizedValue?.ToLowerInvariant();
                        normalizedRef = normalizedRef?.ToLowerInvariant();
                    }

                    // Calculate similarity
                    double similarity = config.CustomSimilarityFunction != null
                        ? config.CustomSimilarityFunction(normalizedValue ?? "", normalizedRef ?? "")
                        : FuzzyMatchingAlgorithms.CalculateSimilarity(normalizedValue ?? "", normalizedRef ?? "", config.Algorithm);

                    fieldScores[targetField] = similarity;
                }

                var result = new CrossValidationResult(fieldScores, config.SimilarityThreshold);
                bool isValid = result.IsValid;

                string errorMessage = config.ErrorMessage ?? $"Cross-validation failed: {result.WorstMatchingField} similarity {result.MinSimilarity:F2} below threshold";
                var error = isValid ? null : new PipelineError(errorMessage, "CrossValidate");

                return new PipelineValue<CrossValidationResult>(result, isValid && pipelineValue.IsValid, pipelineValue.Errors.Concat(error != null ? new[] { error } : Enumerable.Empty<PipelineError>()));
            }
            catch (Exception ex)
            {
                return new PipelineValue<CrossValidationResult>(null, false, pipelineValue.Errors.Concat(new[] { new PipelineError($"Cross-validation error: {ex.Message}", "CrossValidate") }));
            }
        }

        /// <summary>
        /// Performs fuzzy matching against multiple reference values and returns the best match.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a string to match.</param>
        /// <param name="referenceValues">Collection of reference strings to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a tuple of (bestMatch, similarityScore).</returns>
        public static PipelineValue<Tuple<string, double>> FuzzyMatchMany(
            this PipelineValue<string> pipelineValue,
            IEnumerable<string> referenceValues,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<Tuple<string, double>>(null, false, new[] { new PipelineError("PipelineValue is null", "FuzzyMatchMany") });
            }

            if (!pipelineValue.IsValid)
            {
                return new PipelineValue<Tuple<string, double>>(null, false, pipelineValue.Errors);
            }

            try
            {
                string value = pipelineValue.Value;
                if (string.IsNullOrEmpty(value) || referenceValues == null)
                {
                    return new PipelineValue<Tuple<string, double>>(Tuple.Create<string, double>(null, 0.0), false, new[] { new PipelineError("Input value or reference values are null or empty", "FuzzyMatchMany") });
                }

                string normalizedValue = value;
                if (config.NormalizeAddress)
                {
                    normalizedValue = NormalizationUtilities.NormalizeAddress(normalizedValue, config.CaseSensitive == false);
                }
                else if (config.NormalizePhone)
                {
                    normalizedValue = NormalizationUtilities.NormalizePhone(normalizedValue);
                }
                else if (config.NormalizeName)
                {
                    normalizedValue = NormalizationUtilities.NormalizeName(normalizedValue, config.CaseSensitive == false);
                }

                if (!config.CaseSensitive)
                {
                    normalizedValue = normalizedValue.ToLowerInvariant();
                }

                string bestMatch = null;
                double bestSimilarity = 0.0;

                foreach (string refValue in referenceValues)
                {
                    if (string.IsNullOrEmpty(refValue))
                        continue;

                    string normalizedRef = refValue;
                    if (config.NormalizeAddress)
                    {
                        normalizedRef = NormalizationUtilities.NormalizeAddress(normalizedRef, config.CaseSensitive == false);
                    }
                    else if (config.NormalizePhone)
                    {
                        normalizedRef = NormalizationUtilities.NormalizePhone(normalizedRef);
                    }
                    else if (config.NormalizeName)
                    {
                        normalizedRef = NormalizationUtilities.NormalizeName(normalizedRef, config.CaseSensitive == false);
                    }

                    if (!config.CaseSensitive)
                    {
                        normalizedRef = normalizedRef.ToLowerInvariant();
                    }

                    double similarity = config.CustomSimilarityFunction != null
                        ? config.CustomSimilarityFunction(normalizedValue, normalizedRef)
                        : FuzzyMatchingAlgorithms.CalculateSimilarity(normalizedValue, normalizedRef, config.Algorithm);

                    if (similarity > bestSimilarity)
                    {
                        bestSimilarity = similarity;
                        bestMatch = refValue; // Keep original format
                    }
                }

                var result = Tuple.Create(bestMatch, bestSimilarity);
                bool isValid = bestMatch != null && bestSimilarity >= config.SimilarityThreshold;

                if (!isValid && !config.ReturnBestMatch)
                {
                    string errorMessage = config.ErrorMessage ?? $"No match found: best similarity {bestSimilarity:F2} below threshold {config.SimilarityThreshold:F2}";
                    return new PipelineValue<Tuple<string, double>>(result, false, pipelineValue.Errors.Concat(new[] { new PipelineError(errorMessage, "FuzzyMatchMany") }));
                }

                return new PipelineValue<Tuple<string, double>>(result, isValid && pipelineValue.IsValid, pipelineValue.Errors);
            }
            catch (Exception ex)
            {
                return new PipelineValue<Tuple<string, double>>(null, false, pipelineValue.Errors.Concat(new[] { new PipelineError($"Fuzzy match many error: {ex.Message}", "FuzzyMatchMany") }));
            }
        }

        /// <summary>
        /// Performs batch fuzzy matching: compares each string in a collection against reference values.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a collection of strings to match.</param>
        /// <param name="referenceValues">Collection of reference strings to compare against.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a collection of tuples (originalValue, bestMatch, similarityScore).</returns>
        public static PipelineValue<IEnumerable<Tuple<string, string, double>>> FuzzyMatchManyBatch(
            this PipelineValue<IEnumerable<string>> pipelineValue,
            IEnumerable<string> referenceValues,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<IEnumerable<Tuple<string, string, double>>>(null, false, new[] { new PipelineError("PipelineValue is null", "FuzzyMatchManyBatch") });
            }

            if (!pipelineValue.IsValid)
            {
                return new PipelineValue<IEnumerable<Tuple<string, string, double>>>(null, false, pipelineValue.Errors);
            }

            try
            {
                if (pipelineValue.Value == null || referenceValues == null)
                {
                    return new PipelineValue<IEnumerable<Tuple<string, string, double>>>(Enumerable.Empty<Tuple<string, string, double>>(), false, new[] { new PipelineError("Input collection or reference values are null", "FuzzyMatchManyBatch") });
                }

                var results = new List<Tuple<string, string, double>>();

                foreach (string value in pipelineValue.Value)
                {
                    var matchResult = value.FuzzyMatchMany(referenceValues, config);
                    if (matchResult.IsValid && matchResult.Value != null)
                    {
                        results.Add(Tuple.Create(value, matchResult.Value.Item1, matchResult.Value.Item2));
                    }
                    else
                    {
                        results.Add(Tuple.Create<string, string, double>(value, null, 0.0));
                    }
                }

                return new PipelineValue<IEnumerable<Tuple<string, string, double>>>(results, pipelineValue.IsValid, pipelineValue.Errors);
            }
            catch (Exception ex)
            {
                return new PipelineValue<IEnumerable<Tuple<string, string, double>>>(null, false, pipelineValue.Errors.Concat(new[] { new PipelineError($"Batch fuzzy match error: {ex.Message}", "FuzzyMatchManyBatch") }));
            }
        }

        /// <summary>
        /// Performs batch cross-validation: compares each string in a collection against structured reference data.
        /// </summary>
        /// <param name="pipelineValue">The PipelineValue containing a collection of strings to validate.</param>
        /// <param name="referenceData">The structured reference data object.</param>
        /// <param name="fieldMappings">Dictionary mapping field names in referenceData to field names for comparison.</param>
        /// <param name="config">Optional fuzzy matching configuration.</param>
        /// <returns>A PipelineValue containing a collection of CrossValidationResults.</returns>
        public static PipelineValue<IEnumerable<CrossValidationResult>> CrossValidateMany(
            this PipelineValue<IEnumerable<string>> pipelineValue,
            object referenceData,
            Dictionary<string, string> fieldMappings,
            FuzzyMatchingConfig config = null)
        {
            config = config ?? FuzzyMatchingConfig.Default;

            if (pipelineValue == null)
            {
                return new PipelineValue<IEnumerable<CrossValidationResult>>(null, false, new[] { new PipelineError("PipelineValue is null", "CrossValidateMany") });
            }

            if (!pipelineValue.IsValid)
            {
                return new PipelineValue<IEnumerable<CrossValidationResult>>(null, false, pipelineValue.Errors);
            }

            try
            {
                if (pipelineValue.Value == null)
                {
                    return new PipelineValue<IEnumerable<CrossValidationResult>>(Enumerable.Empty<CrossValidationResult>(), false, new[] { new PipelineError("Input collection is null", "CrossValidateMany") });
                }

                var results = new List<CrossValidationResult>();

                foreach (string value in pipelineValue.Value)
                {
                    var validationResult = value.CrossValidate(referenceData, fieldMappings, config);
                    if (validationResult.IsValid && validationResult.Value != null)
                    {
                        results.Add(validationResult.Value);
                    }
                    else
                    {
                        // Create an empty result for failed validations
                        results.Add(new CrossValidationResult(new Dictionary<string, double>(), config.SimilarityThreshold));
                    }
                }

                bool allValid = results.All(r => r.IsValid);
                return new PipelineValue<IEnumerable<CrossValidationResult>>(results, allValid && pipelineValue.IsValid, pipelineValue.Errors);
            }
            catch (Exception ex)
            {
                return new PipelineValue<IEnumerable<CrossValidationResult>>(null, false, pipelineValue.Errors.Concat(new[] { new PipelineError($"Batch cross-validation error: {ex.Message}", "CrossValidateMany") }));
            }
        }

        #endregion
    }
}

