using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines.Tests.Extensions
{
    public class FuzzyMatchingTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            FuzzyMatch_ExactMatch_Passes();
            FuzzyMatch_SimilarMatch_Passes();
            FuzzyMatch_DifferentMatch_Fails();
            FuzzyMatch_WithNormalization_Passes();
            FuzzyMatch_WithCaseInsensitive_Passes();
            FuzzyMatch_OnInvalidPipelineValue_PreservesState();
            FuzzyContains_SubstringFound_Passes();
            FuzzyContains_SubstringNotFound_Fails();
            CorrectTypos_FindsBestMatch_ReturnsCorrected();
            CorrectTypos_NoGoodMatch_ReturnsOriginal();
            CorrectTypos_WithNormalization_Works();
            NormalizeAddress_TransformsCorrectly();
            NormalizePhone_TransformsCorrectly();
            NormalizeName_TransformsCorrectly();
            FuzzyMatchMany_ReturnsBestMatch();
            FuzzyMatchMany_WithThreshold_PassesOrFails();
            CrossValidate_AllFieldsMatch_Passes();
            CrossValidate_SomeFieldsFail_Fails();
            CrossValidate_WithNormalization_Works();
        }

        public void FuzzyMatch_ExactMatch_Passes()
        {
            // Arrange
            string source = "John";

            // Act
            var result = source.FuzzyMatch("John");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Exact match should pass. Expected: true, Actual: {result.IsValid}", true, "Exact match should pass", false, false);
        }

        public void FuzzyMatch_SimilarMatch_Passes()
        {
            // Arrange
            string source = "Johnathon";
            var config = new FuzzyMatchingConfig
            {
                Algorithm = FuzzyAlgorithm.JaroWinkler,
                SimilarityThreshold = 0.7
            };

            // Act
            var result = source.FuzzyMatch("John", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Similar match should pass with low threshold. Expected: true, Actual: {result.IsValid}", true, "Similar match should pass with low threshold", false, false);
        }

        public void FuzzyMatch_DifferentMatch_Fails()
        {
            // Arrange
            string source = "John";
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.9
            };

            // Act
            var result = source.FuzzyMatch("Terry", config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Different match should fail. Expected: false, Actual: {result.IsValid}", true, "Different match should fail", false, false);
        }

        public void FuzzyMatch_WithNormalization_Passes()
        {
            // Arrange
            string source = "123 Main St.";
            var config = new FuzzyMatchingConfig
            {
                NormalizeAddress = true,
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.FuzzyMatch("123 Main Street", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Normalized addresses should match. Expected: true, Actual: {result.IsValid}", true, "Normalized addresses should match", false, false);
        }

        public void FuzzyMatch_WithCaseInsensitive_Passes()
        {
            // Arrange
            string source = "JOHN";
            var config = new FuzzyMatchingConfig
            {
                CaseSensitive = false
            };

            // Act
            var result = source.FuzzyMatch("john", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Case-insensitive match should pass. Expected: true, Actual: {result.IsValid}", true, "Case-insensitive match should pass", false, false);
        }

        public void FuzzyMatch_OnInvalidPipelineValue_PreservesState()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test", false);

            // Act
            var result = pipelineValue.FuzzyMatch("test");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should preserve invalid state. Expected: false, Actual: {result.IsValid}", true, "Should preserve invalid state", false, false);
        }

        public void FuzzyContains_SubstringFound_Passes()
        {
            // Arrange
            string source = "Johnathon Smith";
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.7
            };

            // Act
            var result = source.FuzzyContains("John", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Fuzzy contains should find substring. Expected: true, Actual: {result.IsValid}", true, "Fuzzy contains should find substring", false, false);
        }

        public void FuzzyContains_SubstringNotFound_Fails()
        {
            // Arrange
            string source = "John Smith";
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.9
            };

            // Act
            var result = source.FuzzyContains("Terry", config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Fuzzy contains should fail when not found. Expected: false, Actual: {result.IsValid}", true, "Fuzzy contains should fail when not found", false, false);
        }

        public void CorrectTypos_FindsBestMatch_ReturnsCorrected()
        {
            // Arrange
            string source = "Jonh";
            var correctValues = new[] { "John", "Terry", "Mike" };
            var config = new FuzzyMatchingConfig
            {
                Algorithm = FuzzyAlgorithm.JaroWinkler,
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.CorrectTypos(correctValues, config);

            // Assert
            testing.VerifyExpression(result.Value == "John", $"Should correct to best match. Expected: 'John', Actual: '{result.Value}'", true, "Should correct to best match", false, false);
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }

        public void CorrectTypos_NoGoodMatch_ReturnsOriginal()
        {
            // Arrange
            string source = "Xyz";
            var correctValues = new[] { "John", "Terry", "Mike" };
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.CorrectTypos(correctValues, config);

            // Assert
            testing.VerifyExpression(result.Value == "Xyz", $"Should return original if no good match. Expected: 'Xyz', Actual: '{result.Value}'", true, "Should return original if no good match", false, false);
        }

        public void CorrectTypos_WithNormalization_Works()
        {
            // Arrange
            string source = "123 Main St.";
            var correctValues = new[] { "123 Main Street", "456 Oak Ave" };
            var config = new FuzzyMatchingConfig
            {
                NormalizeAddress = true,
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.CorrectTypos(correctValues, config);

            // Assert
            testing.VerifyExpression(result.Value == "123 Main Street", $"Should normalize and match. Expected: '123 Main Street', Actual: '{result.Value}'", true, "Should normalize and match", false, false);
        }

        public void NormalizeAddress_TransformsCorrectly()
        {
            // Arrange
            string source = "123 Main St.";

            // Act
            var result = source.NormalizeAddress();

            // Assert
            testing.VerifyExpression(result.Value.Contains("street"), $"Should expand abbreviations. Expected: result to contain 'street', Actual: '{result.Value}'", true, "Should expand abbreviations", false, false);
            testing.VerifyExpression(!result.Value.Contains("."), $"Should remove punctuation. Expected: result not to contain '.', Actual: '{result.Value}'", true, "Should remove punctuation", false, false);
        }

        public void NormalizePhone_TransformsCorrectly()
        {
            // Arrange
            string source = "(555) 123-4567";

            // Act
            var result = source.NormalizePhone();

            // Assert
            testing.VerifyExpression(result.Value == "5551234567", $"Should contain only digits. Expected: '5551234567', Actual: '{result.Value}'", true, "Should contain only digits", false, false);
        }

        public void NormalizeName_TransformsCorrectly()
        {
            // Arrange
            string source = "  JOHN   SMITH  ";

            // Act
            var result = source.NormalizeName();

            // Assert
            testing.VerifyExpression(result.Value == "john smith", $"Should normalize whitespace and case. Expected: 'john smith', Actual: '{result.Value}'", true, "Should normalize whitespace and case", false, false);
        }

        public void FuzzyMatchMany_ReturnsBestMatch()
        {
            // Arrange
            string source = "John";
            var references = new[] { "Jon", "Johnathon", "Terry" };
            var config = new FuzzyMatchingConfig
            {
                Algorithm = FuzzyAlgorithm.JaroWinkler,
                SimilarityThreshold = 0.5
            };

            // Act
            var result = source.FuzzyMatchMany(references, config);

            // Assert
            testing.VerifyExpression(result.Value != null, $"Should return a match. Expected: not null, Actual: {result.Value}", true, "Should return a match", false, false);
            testing.VerifyExpression(result.Value.Item2 > 0.0, $"Should have similarity score. Expected: > 0.0, Actual: {result.Value.Item2}", true, "Should have similarity score", false, false);
        }

        public void FuzzyMatchMany_WithThreshold_PassesOrFails()
        {
            // Arrange
            string source = "John";
            var references = new[] { "Terry", "Mike" };
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.FuzzyMatchMany(references, config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should fail when no good match. Expected: false, Actual: {result.IsValid}", true, "Should fail when no good match", false, false);
        }

        public void CrossValidate_AllFieldsMatch_Passes()
        {
            // Arrange
            string source = "John";
            var referenceData = new { Name = "John", Address = "123 Main St" };
            var fieldMappings = new Dictionary<string, string>
            {
                { "Name", "Name" }
            };
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.CrossValidate(referenceData, fieldMappings, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should pass when all fields match. Expected: true, Actual: {result.IsValid}", true, "Should pass when all fields match", false, false);
        }

        public void CrossValidate_SomeFieldsFail_Fails()
        {
            // Arrange
            string source = "Terry";
            var referenceData = new { Name = "John" };
            var fieldMappings = new Dictionary<string, string>
            {
                { "Name", "Name" }
            };
            var config = new FuzzyMatchingConfig
            {
                SimilarityThreshold = 0.9
            };

            // Act
            var result = source.CrossValidate(referenceData, fieldMappings, config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should fail when fields don't match. Expected: false, Actual: {result.IsValid}", true, "Should fail when fields don't match", false, false);
        }

        public void CrossValidate_WithNormalization_Works()
        {
            // Arrange
            string source = "123 Main St.";
            var referenceData = new { Address = "123 Main Street" };
            var fieldMappings = new Dictionary<string, string>
            {
                { "Address", "Address" }
            };
            var config = new FuzzyMatchingConfig
            {
                NormalizeAddress = true,
                SimilarityThreshold = 0.8
            };

            // Act
            var result = source.CrossValidate(referenceData, fieldMappings, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should match with normalization. Expected: true, Actual: {result.IsValid}", true, "Should match with normalization", false, false);
        }
    }
}

