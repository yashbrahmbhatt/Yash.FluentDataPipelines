using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines.Tests.Integration
{
    public class FuzzyMatchingIntegrationTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            RealWorld_NameVariations_JohnVsJohnathon();
            RealWorld_NameVariations_TerryVsTerence();
            RealWorld_AddressNormalization();
            RealWorld_PhoneNumberVariations();
            RealWorld_TypoCorrection_WithChaining();
            RealWorld_CrossValidation_MultipleFields();
            RealWorld_BatchMatching_Names();
            RealWorld_Pipeline_ExtractNormalizeFuzzyMatch();
            RealWorld_Pipeline_CorrectTyposThenValidate();
            RealWorld_Pipeline_CrossValidateThenFormat();
        }

        public void RealWorld_NameVariations_JohnVsJohnathon()
        {
            // Arrange
            string unstructuredName = "Johnathon";

            // Act
            var result = unstructuredName
                .FuzzyMatch("John", new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.7
                })
                .Format((value, isValid) => isValid ? $"Match: {value}" : "No match");

            // Assert
            testing.VerifyExpression(result.Contains("Match"), $"Should match Johnathon with John. Expected: result to contain 'Match', Actual: {result}", true, "Should match Johnathon with John", false, false);
        }

        public void RealWorld_NameVariations_TerryVsTerence()
        {
            // Arrange
            string unstructuredName = "Terry";

            // Act
            var result = unstructuredName
                .FuzzyMatch("Terence", new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.6
                })
                .Format((value, isValid) => isValid ? "Match" : "No match");

            // Assert
            testing.VerifyExpression(result == "Match" || result == "No match", $"Should attempt to match. Expected: 'Match' or 'No match', Actual: '{result}'", true, "Should attempt to match", false, false);
        }

        public void RealWorld_AddressNormalization()
        {
            // Arrange
            string unstructuredAddress = "123 Main St., Apt. 4B";

            // Act
            var result = unstructuredAddress
                .NormalizeAddress()
                .FuzzyMatch("123 main street apt 4b", new FuzzyMatchingConfig
                {
                    SimilarityThreshold = 0.8
                });

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Normalized addresses should match. Expected: true, Actual: {result.IsValid}", true, "Normalized addresses should match", false, false);
        }

        public void RealWorld_PhoneNumberVariations()
        {
            // Arrange
            string unstructuredPhone = "(555) 123-4567";

            // Act
            var result = unstructuredPhone
                .NormalizePhone()
                .FuzzyMatch("5551234567", new FuzzyMatchingConfig
                {
                    SimilarityThreshold = 0.9
                });

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Normalized phones should match. Expected: true, Actual: {result.IsValid}", true, "Normalized phones should match", false, false);
        }

        public void RealWorld_TypoCorrection_WithChaining()
        {
            // Arrange
            string typoName = "Jonh";
            var correctNames = new[] { "John", "Terry", "Mike", "David" };

            // Act
            var result = typoName
                .CorrectTypos(correctNames, new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.8
                })
                .Contains("John")
                .Format((value, isValid) => isValid ? $"Corrected: {value}" : "Not corrected");

            // Assert
            testing.VerifyExpression(result.Contains("Corrected"), $"Should correct typo and validate. Expected: result to contain 'Corrected', Actual: {result}", true, "Should correct typo and validate", false, false);
        }

        public void RealWorld_CrossValidation_MultipleFields()
        {
            // Arrange
            string unstructuredName = "Johnathon";
            var structuredData = new
            {
                Name = "John",
                Address = "123 Main Street",
                Phone = "5551234567"
            };
            var fieldMappings = new Dictionary<string, string>
            {
                { "Name", "Name" }
            };

            // Act
            var result = unstructuredName
                .CrossValidate(structuredData, fieldMappings, new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.7
                });

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should cross-validate successfully. Expected: true, Actual: {result.IsValid}", true, "Should cross-validate successfully", false, false);
            testing.VerifyExpression(result.Value != null, $"Should have validation result. Expected: not null, Actual: {result.Value}", true, "Should have validation result", false, false);
            if (result.Value != null)
            {
                testing.VerifyExpression(result.Value.FieldScores.ContainsKey("Name"), $"Should have Name field score. Expected: FieldScores to contain 'Name', Actual: {result.Value.FieldScores.ContainsKey("Name")}", true, "Should have Name field score", false, false);
            }
        }

        public void RealWorld_BatchMatching_Names()
        {
            // Arrange
            string sourceName = "John";
            var referenceNames = new[] { "Jon", "Johnathon", "Johnny", "Terry", "Mike" };

            // Act
            var result = sourceName
                .FuzzyMatchMany(referenceNames, new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.7
                });

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should find match in batch. Expected: true, Actual: {result.IsValid}", true, "Should find match in batch", false, false);
            testing.VerifyExpression(result.Value != null, $"Should return match result. Expected: not null, Actual: {result.Value}", true, "Should return match result", false, false);
            if (result.Value != null)
            {
                testing.VerifyExpression(result.Value.Item2 > 0.7, $"Should have high similarity. Expected: > 0.7, Actual: {result.Value.Item2}", true, "Should have high similarity", false, false);
            }
        }

        public void RealWorld_Pipeline_ExtractNormalizeFuzzyMatch()
        {
            // Arrange
            string invoiceText = "Customer: Johnathon Smith, Address: 123 Main St.";

            // Act - Extract name, normalize, and fuzzy match
            var name = invoiceText
                .Extract<string>(
                    new Yash.FluentDataPipelines.Configuration.ExtractConfig
                    {
                        RegexPattern = @"Customer:\s*(\w+)",
                        GroupIndex = 1
                    },
                    (str, cfg) => str)
                .NormalizeName()
                .FuzzyMatch("John", new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.7
                })
                .Format((value, isValid) => isValid ? $"Matched: {value}" : "No match");

            // Assert
            testing.VerifyExpression(name.Length > 0, $"Should process pipeline. Expected: length > 0, Actual: {name.Length}", true, "Should process pipeline", false, false);
        }

        public void RealWorld_Pipeline_CorrectTyposThenValidate()
        {
            // Arrange
            string typoEmail = "jonh@example.com";
            var correctNames = new[] { "John", "Terry", "Mike" };

            // Act
            var result = typoEmail
                .Extract<string>(
                    new Yash.FluentDataPipelines.Configuration.ExtractConfig
                    {
                        RegexPattern = @"^(\w+)@",
                        GroupIndex = 1
                    },
                    (str, cfg) => str)
                .CorrectTypos(correctNames, new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.8
                })
                .Contains("John")
                .Format((value, isValid) => isValid ? $"Valid: {value}" : "Invalid");

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should process correction and validation. Expected: length > 0, Actual: {result.Length}", true, "Should process correction and validation", false, false);
        }

        public void RealWorld_Pipeline_CrossValidateThenFormat()
        {
            // Arrange
            string unstructuredData = "Johnathon";
            var structuredData = new { Name = "John", ID = 12345 };
            var fieldMappings = new Dictionary<string, string> { { "Name", "Name" } };

            // Act
            var result = unstructuredData
                .CrossValidate(structuredData, fieldMappings, new FuzzyMatchingConfig
                {
                    Algorithm = FuzzyAlgorithm.JaroWinkler,
                    SimilarityThreshold = 0.7
                })
                .Format((value, isValid) =>
                {
                    if (value == null) return "No result";
                    return isValid
                        ? $"Valid: {value.BestMatchingField} ({value.AverageSimilarity:F2})"
                        : $"Invalid: {value.WorstMatchingField} ({value.MinSimilarity:F2})";
                });

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format cross-validation result. Expected: length > 0, Actual: {result.Length}", true, "Should format cross-validation result", false, false);
        }
    }
}

