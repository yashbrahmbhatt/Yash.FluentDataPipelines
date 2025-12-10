using System;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Tests.Core
{
    public class FuzzyMatchingAlgorithmsTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            LevenshteinDistance_ExactMatch_ReturnsZero();
            LevenshteinDistance_OneCharacterDifference_ReturnsOne();
            LevenshteinDistance_CompletelyDifferent_ReturnsLength();
            LevenshteinSimilarity_ExactMatch_ReturnsOne();
            LevenshteinSimilarity_NoMatch_ReturnsZero();
            LevenshteinSimilarity_PartialMatch_ReturnsBetweenZeroAndOne();
            JaroSimilarity_ExactMatch_ReturnsOne();
            JaroSimilarity_NoMatch_ReturnsZero();
            JaroSimilarity_Transpositions_HandledCorrectly();
            JaroWinklerSimilarity_ExactMatch_ReturnsOne();
            JaroWinklerSimilarity_PrefixMatch_HigherScore();
            JaroWinklerSimilarity_NoPrefixMatch_SameAsJaro();
            CalculateSimilarity_Levenshtein_ReturnsCorrectValue();
            CalculateSimilarity_Jaro_ReturnsCorrectValue();
            CalculateSimilarity_JaroWinkler_ReturnsCorrectValue();
            CalculateSimilarity_NullStrings_HandledCorrectly();
            CalculateSimilarity_EmptyStrings_ReturnsOne();
        }

        public void LevenshteinDistance_ExactMatch_ReturnsZero()
        {
            // Arrange & Act
            int distance = FuzzyMatchingAlgorithms.LevenshteinDistance("test", "test");

            // Assert
            testing.VerifyExpression(distance == 0, $"Exact match should have distance 0. Expected: 0, Actual: {distance}", true, "0 - Exact match should have distance 0", false, false);
        }

        public void LevenshteinDistance_OneCharacterDifference_ReturnsOne()
        {
            // Arrange & Act
            int distance = FuzzyMatchingAlgorithms.LevenshteinDistance("test", "best");

            // Assert
            testing.VerifyExpression(distance == 1, $"One character difference should have distance 1. Expected: 1, Actual: {distance}", true, "1 - One character difference should have distance 1", false, false);
        }

        public void LevenshteinDistance_CompletelyDifferent_ReturnsLength()
        {
            // Arrange & Act
            int distance = FuzzyMatchingAlgorithms.LevenshteinDistance("abc", "xyz");

            // Assert
            testing.VerifyExpression(distance == 3, $"Completely different strings should have distance equal to length. Expected: 3, Actual: {distance}", true, "3 - Completely different strings should have distance equal to length", false, false);
        }

        public void LevenshteinSimilarity_ExactMatch_ReturnsOne()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.LevenshteinSimilarity("test", "test");

            // Assert
            testing.VerifyExpression(similarity == 1.0, $"Exact match should have similarity 1.0. Expected: 1.0, Actual: {similarity}", true, "1.0 - Exact match should have similarity 1.0", false, false);
        }

        public void LevenshteinSimilarity_NoMatch_ReturnsZero()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.LevenshteinSimilarity("abc", "xyz");

            // Assert
            testing.VerifyExpression(similarity == 0.0, $"No match should have similarity 0.0. Expected: 0.0, Actual: {similarity}", true, "0.0 - No match should have similarity 0.0", false, false);
        }

        public void LevenshteinSimilarity_PartialMatch_ReturnsBetweenZeroAndOne()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.LevenshteinSimilarity("John", "Jon");

            // Assert
            testing.VerifyExpression(similarity > 0.0 && similarity < 1.0, $"Partial match should have similarity between 0 and 1. Expected: > 0.0 and < 1.0, Actual: {similarity}", true, "Partial match should have similarity between 0 and 1", false, false);
        }

        public void JaroSimilarity_ExactMatch_ReturnsOne()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.JaroSimilarity("test", "test");

            // Assert
            testing.VerifyExpression(similarity == 1.0, $"Exact match should have similarity 1.0. Expected: 1.0, Actual: {similarity}", true, "1.0 - Exact match should have similarity 1.0", false, false);
        }

        public void JaroSimilarity_NoMatch_ReturnsZero()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.JaroSimilarity("abc", "xyz");

            // Assert
            testing.VerifyExpression(similarity == 0.0, $"No match should have similarity 0.0. Expected: 0.0, Actual: {similarity}", true, "0.0 - No match should have similarity 0.0", false, false);
        }

        public void JaroSimilarity_Transpositions_HandledCorrectly()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.JaroSimilarity("John", "Jonh");

            // Assert
            testing.VerifyExpression(similarity > 0.0, $"Transpositions should be handled. Expected: > 0.0, Actual: {similarity}", true, "Transpositions should be handled", false, false);
        }

        public void JaroWinklerSimilarity_ExactMatch_ReturnsOne()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.JaroWinklerSimilarity("test", "test");

            // Assert
            testing.VerifyExpression(similarity == 1.0, $"Exact match should have similarity 1.0. Expected: 1.0, Actual: {similarity}", true, "1.0 - Exact match should have similarity 1.0", false, false);
        }

        public void JaroWinklerSimilarity_PrefixMatch_HigherScore()
        {
            // Arrange
            string str1 = "Johnathon";
            string str2 = "John";

            // Act
            double jaro = FuzzyMatchingAlgorithms.JaroSimilarity(str1, str2);
            double jaroWinkler = FuzzyMatchingAlgorithms.JaroWinklerSimilarity(str1, str2);

            // Assert
            testing.VerifyExpression(jaroWinkler >= jaro, $"Jaro-Winkler should give higher score for prefix matches. Expected: >= {jaro}, Actual: {jaroWinkler}", true, "Jaro-Winkler should give higher score for prefix matches", false, false);
        }

        public void JaroWinklerSimilarity_NoPrefixMatch_SameAsJaro()
        {
            // Arrange
            string str1 = "abc";
            string str2 = "xyz";

            // Act
            double jaro = FuzzyMatchingAlgorithms.JaroSimilarity(str1, str2);
            double jaroWinkler = FuzzyMatchingAlgorithms.JaroWinklerSimilarity(str1, str2);

            // Assert
            testing.VerifyExpression(Math.Abs(jaroWinkler - jaro) < 0.01, $"Jaro-Winkler should be same as Jaro when no prefix match. Expected: difference < 0.01, Actual: {Math.Abs(jaroWinkler - jaro)}", true, "Jaro-Winkler should be same as Jaro when no prefix match", false, false);
        }

        public void CalculateSimilarity_Levenshtein_ReturnsCorrectValue()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.CalculateSimilarity("test", "best", FuzzyAlgorithm.Levenshtein);

            // Assert
            testing.VerifyExpression(similarity > 0.0 && similarity < 1.0, $"Should return similarity between 0 and 1. Expected: > 0.0 and < 1.0, Actual: {similarity}", true, "Should return similarity between 0 and 1", false, false);
        }

        public void CalculateSimilarity_Jaro_ReturnsCorrectValue()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.CalculateSimilarity("test", "best", FuzzyAlgorithm.Jaro);

            // Assert
            testing.VerifyExpression(similarity > 0.0 && similarity < 1.0, $"Should return similarity between 0 and 1. Expected: > 0.0 and < 1.0, Actual: {similarity}", true, "Should return similarity between 0 and 1", false, false);
        }

        public void CalculateSimilarity_JaroWinkler_ReturnsCorrectValue()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.CalculateSimilarity("Johnathon", "John", FuzzyAlgorithm.JaroWinkler);

            // Assert
            testing.VerifyExpression(similarity > 0.0 && similarity <= 1.0, $"Should return similarity between 0 and 1. Expected: > 0.0 and <= 1.0, Actual: {similarity}", true, "Should return similarity between 0 and 1", false, false);
        }

        public void CalculateSimilarity_NullStrings_HandledCorrectly()
        {
            // Arrange & Act
            double similarity1 = FuzzyMatchingAlgorithms.CalculateSimilarity(null, null, FuzzyAlgorithm.Levenshtein);
            double similarity2 = FuzzyMatchingAlgorithms.CalculateSimilarity("test", null, FuzzyAlgorithm.Levenshtein);
            double similarity3 = FuzzyMatchingAlgorithms.CalculateSimilarity(null, "test", FuzzyAlgorithm.Levenshtein);

            // Assert
            testing.VerifyExpression(similarity1 == 1.0, $"Both null should return 1.0. Expected: 1.0, Actual: {similarity1}", true, "1.0 - Both null should return 1.0", false, false);
            testing.VerifyExpression(similarity2 == 0.0, $"One null should return 0.0. Expected: 0.0, Actual: {similarity2}", true, "0.0 - One null should return 0.0", false, false);
            testing.VerifyExpression(similarity3 == 0.0, $"One null should return 0.0. Expected: 0.0, Actual: {similarity3}", true, "0.0 - One null should return 0.0", false, false);
        }

        public void CalculateSimilarity_EmptyStrings_ReturnsOne()
        {
            // Arrange & Act
            double similarity = FuzzyMatchingAlgorithms.CalculateSimilarity("", "", FuzzyAlgorithm.Levenshtein);

            // Assert
            testing.VerifyExpression(similarity == 1.0, $"Both empty should return 1.0. Expected: 1.0, Actual: {similarity}", true, "1.0 - Both empty should return 1.0", false, false);
        }
    }
}

