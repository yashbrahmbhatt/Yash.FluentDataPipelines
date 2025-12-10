using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Yash.FluentDataPipelines.Core
{
    /// <summary>
    /// Provides utilities for normalizing strings before comparison.
    /// </summary>
    public static class NormalizationUtilities
    {
        private static readonly Dictionary<string, string> AddressAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "St", "Street" },
            { "St.", "Street" },
            { "Ave", "Avenue" },
            { "Ave.", "Avenue" },
            { "Rd", "Road" },
            { "Rd.", "Road" },
            { "Blvd", "Boulevard" },
            { "Blvd.", "Boulevard" },
            { "Dr", "Drive" },
            { "Dr.", "Drive" },
            { "Ln", "Lane" },
            { "Ln.", "Lane" },
            { "Ct", "Court" },
            { "Ct.", "Court" },
            { "Pl", "Place" },
            { "Pl.", "Place" },
            { "Pkwy", "Parkway" },
            { "Pkwy.", "Parkway" },
            { "Hwy", "Highway" },
            { "Hwy.", "Highway" }
        };

        private static readonly HashSet<string> CommonWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "The", "A", "An", "And", "Or", "But", "Of", "In", "On", "At", "To", "For"
        };

        /// <summary>
        /// Normalizes an address string for comparison.
        /// </summary>
        /// <param name="address">The address string to normalize.</param>
        /// <param name="toLowercase">Whether to convert to lowercase.</param>
        /// <param name="removeCommonWords">Whether to remove common words (The, A, An, etc.).</param>
        /// <returns>Normalized address string.</returns>
        public static string NormalizeAddress(string address, bool toLowercase = true, bool removeCommonWords = false)
        {
            if (string.IsNullOrEmpty(address))
                return address ?? string.Empty;

            string normalized = address;

            // Remove punctuation (keep spaces and alphanumeric)
            normalized = Regex.Replace(normalized, @"[^\w\s]", " ");

            // Normalize whitespace
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

            // Expand abbreviations
            var words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                if (AddressAbbreviations.TryGetValue(words[i], out string expanded))
                {
                    words[i] = expanded;
                }
            }
            normalized = string.Join(" ", words);

            // Remove common words if requested
            if (removeCommonWords)
            {
                words = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                words = words.Where(w => !CommonWords.Contains(w)).ToArray();
                normalized = string.Join(" ", words);
            }

            // Convert to lowercase if requested
            if (toLowercase)
            {
                normalized = normalized.ToLowerInvariant();
            }

            return normalized.Trim();
        }

        /// <summary>
        /// Normalizes a phone number string for comparison.
        /// </summary>
        /// <param name="phone">The phone number string to normalize.</param>
        /// <param name="removeExtensions">Whether to remove extensions (e.g., "x123", "ext123").</param>
        /// <returns>Normalized phone number (digits only by default).</returns>
        public static string NormalizePhone(string phone, bool removeExtensions = true)
        {
            if (string.IsNullOrEmpty(phone))
                return phone ?? string.Empty;

            string normalized = phone;

            // Remove extensions if requested
            if (removeExtensions)
            {
                normalized = Regex.Replace(normalized, @"\s*(?:x|ext|extension)[\s:]*\d+", "", RegexOptions.IgnoreCase);
            }

            // Remove all non-digit characters
            normalized = Regex.Replace(normalized, @"\D", "");

            return normalized;
        }

        /// <summary>
        /// Normalizes a name string for comparison.
        /// </summary>
        /// <param name="name">The name string to normalize.</param>
        /// <param name="toLowercase">Whether to convert to lowercase.</param>
        /// <param name="removeTitles">Whether to remove titles (Mr., Mrs., Dr., etc.).</param>
        /// <returns>Normalized name string.</returns>
        public static string NormalizeName(string name, bool toLowercase = true, bool removeTitles = false)
        {
            if (string.IsNullOrEmpty(name))
                return name ?? string.Empty;

            string normalized = name;

            // Remove titles if requested
            if (removeTitles)
            {
                normalized = Regex.Replace(normalized, @"^(Mr|Mrs|Ms|Miss|Dr|Doctor|Prof|Professor)\.?\s+", "", RegexOptions.IgnoreCase);
            }

            // Normalize whitespace
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

            // Convert to lowercase if requested
            if (toLowercase)
            {
                normalized = normalized.ToLowerInvariant();
            }

            return normalized.Trim();
        }

        /// <summary>
        /// Applies generic string normalization with configurable options.
        /// </summary>
        /// <param name="input">The string to normalize.</param>
        /// <param name="toLowercase">Whether to convert to lowercase.</param>
        /// <param name="removePunctuation">Whether to remove punctuation.</param>
        /// <param name="normalizeWhitespace">Whether to normalize whitespace.</param>
        /// <param name="trim">Whether to trim the result.</param>
        /// <returns>Normalized string.</returns>
        public static string NormalizeString(string input, bool toLowercase = true, bool removePunctuation = false, bool normalizeWhitespace = true, bool trim = true)
        {
            if (string.IsNullOrEmpty(input))
                return input ?? string.Empty;

            string normalized = input;

            if (removePunctuation)
            {
                normalized = Regex.Replace(normalized, @"[^\w\s]", " ");
            }

            if (normalizeWhitespace)
            {
                normalized = Regex.Replace(normalized, @"\s+", " ");
            }

            if (toLowercase)
            {
                normalized = normalized.ToLowerInvariant();
            }

            if (trim)
            {
                normalized = normalized.Trim();
            }

            return normalized;
        }
    }
}

