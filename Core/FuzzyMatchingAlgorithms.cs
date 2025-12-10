using System;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Provides fuzzy string matching algorithms.
    /// </summary>
    public static class FuzzyMatchingAlgorithms
    {
        /// <summary>
        /// Calculates the similarity between two strings using the specified algorithm.
        /// </summary>
        /// <param name="str1">First string to compare.</param>
        /// <param name="str2">Second string to compare.</param>
        /// <param name="algorithm">The algorithm to use.</param>
        /// <returns>Similarity score between 0.0 (no match) and 1.0 (exact match).</returns>
        public static double CalculateSimilarity(string str1, string str2, Configuration.FuzzyAlgorithm algorithm)
        {
            if (str1 == null && str2 == null) return 1.0;
            if (str1 == null || str2 == null) return 0.0;
            if (str1 == str2) return 1.0;
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return 1.0;
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) return 0.0;

            switch (algorithm)
            {
                case Configuration.FuzzyAlgorithm.Levenshtein:
                    return LevenshteinSimilarity(str1, str2);

                case Configuration.FuzzyAlgorithm.Jaro:
                    return JaroSimilarity(str1, str2);

                case Configuration.FuzzyAlgorithm.JaroWinkler:
                    return JaroWinklerSimilarity(str1, str2);

                default:
                    throw new ArgumentException($"Unknown algorithm: {algorithm}", nameof(algorithm));
            }
        }

        /// <summary>
        /// Calculates Levenshtein distance-based similarity.
        /// </summary>
        /// <param name="str1">First string.</param>
        /// <param name="str2">Second string.</param>
        /// <returns>Similarity score between 0.0 and 1.0.</returns>
        public static double LevenshteinSimilarity(string str1, string str2)
        {
            if (str1 == str2) return 1.0;
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) return 0.0;

            int maxLength = Math.Max(str1.Length, str2.Length);
            if (maxLength == 0) return 1.0;

            int distance = LevenshteinDistance(str1, str2);
            return 1.0 - ((double)distance / maxLength);
        }

        /// <summary>
        /// Calculates the Levenshtein distance (edit distance) between two strings.
        /// </summary>
        /// <param name="str1">First string.</param>
        /// <param name="str2">Second string.</param>
        /// <returns>The edit distance (number of character operations needed).</returns>
        public static int LevenshteinDistance(string str1, string str2)
        {
            if (str1 == str2) return 0;
            if (string.IsNullOrEmpty(str1)) return str2?.Length ?? 0;
            if (string.IsNullOrEmpty(str2)) return str1.Length;

            int n = str1.Length;
            int m = str2.Length;
            int[,] d = new int[n + 1, m + 1];

            // Initialize first row and column
            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            // Fill the matrix
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (str1[i - 1] == str2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[n, m];
        }

        /// <summary>
        /// Calculates Jaro similarity between two strings.
        /// </summary>
        /// <param name="str1">First string.</param>
        /// <param name="str2">Second string.</param>
        /// <returns>Similarity score between 0.0 and 1.0.</returns>
        public static double JaroSimilarity(string str1, string str2)
        {
            if (str1 == str2) return 1.0;
            if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) return 0.0;

            int len1 = str1.Length;
            int len2 = str2.Length;

            if (len1 == 0 && len2 == 0) return 1.0;
            if (len1 == 0 || len2 == 0) return 0.0;

            // Match window: floor(max(len1, len2) / 2) - 1
            int matchWindow = Math.Max(len1, len2) / 2 - 1;
            if (matchWindow < 0) matchWindow = 0;

            bool[] str1Matches = new bool[len1];
            bool[] str2Matches = new bool[len2];

            int matches = 0;
            int transpositions = 0;

            // Find matches
            for (int i = 0; i < len1; i++)
            {
                int start = Math.Max(0, i - matchWindow);
                int end = Math.Min(i + matchWindow + 1, len2);

                for (int j = start; j < end; j++)
                {
                    if (str2Matches[j] || str1[i] != str2[j])
                        continue;

                    str1Matches[i] = true;
                    str2Matches[j] = true;
                    matches++;
                    break;
                }
            }

            if (matches == 0) return 0.0;

            // Find transpositions
            int k = 0;
            for (int i = 0; i < len1; i++)
            {
                if (!str1Matches[i]) continue;

                while (!str2Matches[k]) k++;

                if (str1[i] != str2[k])
                    transpositions++;

                k++;
            }

            double jaro = ((double)matches / len1 + (double)matches / len2 + (double)(matches - transpositions / 2.0) / matches) / 3.0;
            return jaro;
        }

        /// <summary>
        /// Calculates Jaro-Winkler similarity between two strings.
        /// This is an enhancement of Jaro that gives higher weight to prefix matches.
        /// </summary>
        /// <param name="str1">First string.</param>
        /// <param name="str2">Second string.</param>
        /// <param name="prefixLength">Length of common prefix to consider. Default is 4.</param>
        /// <param name="scalingFactor">Scaling factor for prefix bonus. Default is 0.1.</param>
        /// <returns>Similarity score between 0.0 and 1.0.</returns>
        public static double JaroWinklerSimilarity(string str1, string str2, int prefixLength = 4, double scalingFactor = 0.1)
        {
            double jaro = JaroSimilarity(str1, str2);

            if (jaro < 0.7) return jaro;

            // Find common prefix length (up to prefixLength)
            int commonPrefix = 0;
            int maxPrefix = Math.Min(Math.Min(prefixLength, str1.Length), str2.Length);
            for (int i = 0; i < maxPrefix; i++)
            {
                if (str1[i] == str2[i])
                    commonPrefix++;
                else
                    break;
            }

            return jaro + (scalingFactor * commonPrefix * (1 - jaro));
        }
    }
}

