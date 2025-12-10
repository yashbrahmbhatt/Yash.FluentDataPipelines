using System;
using System.Collections.Generic;
using System.Linq;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Represents the result of a cross-validation operation between structured and unstructured data.
    /// </summary>
    public class CrossValidationResult
    {
        /// <summary>
        /// Gets a dictionary of field-level validation results.
        /// Key is the field name, value is the similarity score (0.0 to 1.0).
        /// </summary>
        public Dictionary<string, double> FieldScores { get; }

        /// <summary>
        /// Gets a dictionary indicating which fields passed validation.
        /// Key is the field name, value indicates if the field matched above threshold.
        /// </summary>
        public Dictionary<string, bool> FieldMatches { get; }

        /// <summary>
        /// Gets the overall validation result (true if all fields matched above threshold).
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Gets the minimum similarity score across all fields.
        /// </summary>
        public double MinSimilarity { get; }

        /// <summary>
        /// Gets the maximum similarity score across all fields.
        /// </summary>
        public double MaxSimilarity { get; }

        /// <summary>
        /// Gets the average similarity score across all fields.
        /// </summary>
        public double AverageSimilarity { get; }

        /// <summary>
        /// Gets the best matching field name (highest similarity score).
        /// </summary>
        public string BestMatchingField { get; }

        /// <summary>
        /// Gets the worst matching field name (lowest similarity score).
        /// </summary>
        public string WorstMatchingField { get; }

        /// <summary>
        /// Initializes a new instance of the CrossValidationResult class.
        /// </summary>
        /// <param name="fieldScores">Dictionary of field names and their similarity scores.</param>
        /// <param name="threshold">The similarity threshold used for validation.</param>
        public CrossValidationResult(Dictionary<string, double> fieldScores, double threshold)
        {
            FieldScores = fieldScores ?? new Dictionary<string, double>();
            FieldMatches = new Dictionary<string, bool>();

            if (FieldScores.Count == 0)
            {
                IsValid = false;
                MinSimilarity = 0.0;
                MaxSimilarity = 0.0;
                AverageSimilarity = 0.0;
                BestMatchingField = null;
                WorstMatchingField = null;
                return;
            }

            // Calculate field matches
            foreach (var kvp in FieldScores)
            {
                FieldMatches[kvp.Key] = kvp.Value >= threshold;
            }

            // Calculate overall validity
            IsValid = FieldMatches.Values.All(match => match);

            // Calculate statistics
            var scores = FieldScores.Values.ToList();
            MinSimilarity = scores.Min();
            MaxSimilarity = scores.Max();
            AverageSimilarity = scores.Average();

            // Find best and worst matching fields
            BestMatchingField = FieldScores.OrderByDescending(kvp => kvp.Value).First().Key;
            WorstMatchingField = FieldScores.OrderBy(kvp => kvp.Value).First().Key;
        }

        /// <summary>
        /// Gets the similarity score for a specific field.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The similarity score, or 0.0 if the field was not validated.</returns>
        public double GetFieldScore(string fieldName)
        {
            return FieldScores.TryGetValue(fieldName, out double score) ? score : 0.0;
        }

        /// <summary>
        /// Gets whether a specific field matched above the threshold.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>True if the field matched, false otherwise.</returns>
        public bool GetFieldMatch(string fieldName)
        {
            return FieldMatches.TryGetValue(fieldName, out bool match) && match;
        }
    }
}

