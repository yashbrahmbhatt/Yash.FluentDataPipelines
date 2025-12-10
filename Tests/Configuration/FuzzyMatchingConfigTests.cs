using System;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;

namespace Yash.FluentDataPipelines.Tests.Configuration
{
    public class FuzzyMatchingConfigTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Default_Config_HasSensibleDefaults();
            Default_Config_CanBeModified();
            Algorithm_CanBeSet();
            SimilarityThreshold_CanBeSet();
            CaseSensitive_CanBeSet();
            MaxEditDistance_CanBeSet();
            NormalizationOptions_CanBeSet();
            CustomSimilarityFunction_CanBeSet();
        }

        public void Default_Config_HasSensibleDefaults()
        {
            // Arrange & Act
            var config = FuzzyMatchingConfig.Default;

            // Assert
            testing.VerifyExpression(config.Algorithm == FuzzyAlgorithm.JaroWinkler, $"Default algorithm should be JaroWinkler. Expected: {FuzzyAlgorithm.JaroWinkler}, Actual: {config.Algorithm}", true, "Default algorithm should be JaroWinkler", false, false);
            testing.VerifyExpression(config.SimilarityThreshold == 0.8, $"Default threshold should be 0.8. Expected: 0.8, Actual: {config.SimilarityThreshold}", true, "Default threshold should be 0.8", false, false);
            testing.VerifyExpression(config.CaseSensitive == false, $"Default should be case-insensitive. Expected: false, Actual: {config.CaseSensitive}", true, "Default should be case-insensitive", false, false);
        }

        public void Default_Config_CanBeModified()
        {
            // Arrange
            var config = FuzzyMatchingConfig.Default;

            // Act
            config.Algorithm = FuzzyAlgorithm.Levenshtein;
            config.SimilarityThreshold = 0.9;

            // Assert
            testing.VerifyExpression(config.Algorithm == FuzzyAlgorithm.Levenshtein, $"Should be able to modify algorithm. Expected: {FuzzyAlgorithm.Levenshtein}, Actual: {config.Algorithm}", true, "Should be able to modify algorithm", false, false);
            testing.VerifyExpression(config.SimilarityThreshold == 0.9, $"Should be able to modify threshold. Expected: 0.9, Actual: {config.SimilarityThreshold}", true, "Should be able to modify threshold", false, false);
        }

        public void Algorithm_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.Algorithm = FuzzyAlgorithm.Levenshtein;
            var config2 = new FuzzyMatchingConfig { Algorithm = FuzzyAlgorithm.Jaro };

            // Assert
            testing.VerifyExpression(config.Algorithm == FuzzyAlgorithm.Levenshtein, $"Should set Levenshtein. Expected: {FuzzyAlgorithm.Levenshtein}, Actual: {config.Algorithm}", true, "Should set Levenshtein", false, false);
            testing.VerifyExpression(config2.Algorithm == FuzzyAlgorithm.Jaro, $"Should set Jaro. Expected: {FuzzyAlgorithm.Jaro}, Actual: {config2.Algorithm}", true, "Should set Jaro", false, false);
        }

        public void SimilarityThreshold_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.SimilarityThreshold = 0.95;

            // Assert
            testing.VerifyExpression(config.SimilarityThreshold == 0.95, $"Should set threshold. Expected: 0.95, Actual: {config.SimilarityThreshold}", true, "Should set threshold", false, false);
        }

        public void CaseSensitive_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.CaseSensitive = true;

            // Assert
            testing.VerifyExpression(config.CaseSensitive == true, $"Should set case sensitivity. Expected: true, Actual: {config.CaseSensitive}", true, "Should set case sensitivity", false, false);
        }

        public void MaxEditDistance_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.MaxEditDistance = 3;

            // Assert
            testing.VerifyExpression(config.MaxEditDistance == 3, $"Should set max edit distance. Expected: 3, Actual: {config.MaxEditDistance}", true, "Should set max edit distance", false, false);
        }

        public void NormalizationOptions_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.NormalizeAddress = true;
            config.NormalizePhone = true;
            config.NormalizeName = true;

            // Assert
            testing.VerifyExpression(config.NormalizeAddress == true, $"Should set address normalization. Expected: true, Actual: {config.NormalizeAddress}", true, "Should set address normalization", false, false);
            testing.VerifyExpression(config.NormalizePhone == true, $"Should set phone normalization. Expected: true, Actual: {config.NormalizePhone}", true, "Should set phone normalization", false, false);
            testing.VerifyExpression(config.NormalizeName == true, $"Should set name normalization. Expected: true, Actual: {config.NormalizeName}", true, "Should set name normalization", false, false);
        }

        public void CustomSimilarityFunction_CanBeSet()
        {
            // Arrange
            var config = new FuzzyMatchingConfig();

            // Act
            config.CustomSimilarityFunction = (s1, s2) => s1 == s2 ? 1.0 : 0.0;

            // Assert
            testing.VerifyExpression(config.CustomSimilarityFunction != null, $"Should set custom function. Expected: not null, Actual: {config.CustomSimilarityFunction != null}", true, "Should set custom function", false, false);
            testing.VerifyExpression(config.CustomSimilarityFunction("test", "test") == 1.0, $"Custom function should work. Expected: 1.0, Actual: {config.CustomSimilarityFunction("test", "test")}", true, "Custom function should work", false, false);
        }
    }
}

