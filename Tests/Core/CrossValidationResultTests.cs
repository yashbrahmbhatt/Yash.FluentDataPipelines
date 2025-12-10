using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Tests.Core
{
    public class CrossValidationResultTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Constructor_WithAllFieldsAboveThreshold_IsValid();
            Constructor_WithSomeFieldsBelowThreshold_IsInvalid();
            Constructor_WithEmptyScores_IsInvalid();
            GetFieldScore_ReturnsCorrectScore();
            GetFieldScore_NonExistentField_ReturnsZero();
            GetFieldMatch_ReturnsCorrectMatch();
            MinMaxAverage_CalculatedCorrectly();
            BestWorstMatchingField_IdentifiedCorrectly();
        }

        public void Constructor_WithAllFieldsAboveThreshold_IsValid()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Name", 0.9 },
                { "Address", 0.85 },
                { "Phone", 0.95 }
            };

            // Act
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid when all fields above threshold. Expected: true, Actual: {result.IsValid}", true, "Should be valid when all fields above threshold", false, false);
        }

        public void Constructor_WithSomeFieldsBelowThreshold_IsInvalid()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Name", 0.9 },
                { "Address", 0.5 },
                { "Phone", 0.95 }
            };

            // Act
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid when some fields below threshold. Expected: false, Actual: {result.IsValid}", true, "Should be invalid when some fields below threshold", false, false);
        }

        public void Constructor_WithEmptyScores_IsInvalid()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>();

            // Act
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid with empty scores. Expected: false, Actual: {result.IsValid}", true, "Should be invalid with empty scores", false, false);
        }

        public void GetFieldScore_ReturnsCorrectScore()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Name", 0.9 },
                { "Address", 0.85 }
            };
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Act
            double score = result.GetFieldScore("Name");

            // Assert
            testing.VerifyExpression(score == 0.9, $"Should return correct field score. Expected: 0.9, Actual: {score}", true, "Should return correct field score", false, false);
        }

        public void GetFieldScore_NonExistentField_ReturnsZero()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Name", 0.9 }
            };
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Act
            double score = result.GetFieldScore("NonExistent");

            // Assert
            testing.VerifyExpression(score == 0.0, $"Should return 0.0 for non-existent field. Expected: 0.0, Actual: {score}", true, "Should return 0.0 for non-existent field", false, false);
        }

        public void GetFieldMatch_ReturnsCorrectMatch()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Name", 0.9 },
                { "Address", 0.5 }
            };
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Act
            bool nameMatch = result.GetFieldMatch("Name");
            bool addressMatch = result.GetFieldMatch("Address");

            // Assert
            testing.VerifyExpression(nameMatch == true, $"Name should match. Expected: true, Actual: {nameMatch}", true, "Name should match", false, false);
            testing.VerifyExpression(addressMatch == false, $"Address should not match. Expected: false, Actual: {addressMatch}", true, "Address should not match", false, false);
        }

        public void MinMaxAverage_CalculatedCorrectly()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Field1", 0.5 },
                { "Field2", 0.8 },
                { "Field3", 0.9 }
            };

            // Act
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Assert
            testing.VerifyExpression(result.MinSimilarity == 0.5, $"Min should be 0.5. Expected: 0.5, Actual: {result.MinSimilarity}", true, "Min should be 0.5", false, false);
            testing.VerifyExpression(result.MaxSimilarity == 0.9, $"Max should be 0.9. Expected: 0.9, Actual: {result.MaxSimilarity}", true, "Max should be 0.9", false, false);
            testing.VerifyExpression(Math.Abs(result.AverageSimilarity - 0.733) < 0.01, $"Average should be approximately 0.733. Expected: ~0.733, Actual: {result.AverageSimilarity}", true, "Average should be approximately 0.733", false, false);
        }

        public void BestWorstMatchingField_IdentifiedCorrectly()
        {
            // Arrange
            var fieldScores = new Dictionary<string, double>
            {
                { "Field1", 0.5 },
                { "Field2", 0.8 },
                { "Field3", 0.9 }
            };

            // Act
            var result = new CrossValidationResult(fieldScores, 0.8);

            // Assert
            testing.VerifyExpression(result.BestMatchingField == "Field3", $"Best matching field should be Field3. Expected: 'Field3', Actual: '{result.BestMatchingField}'", true, "Best matching field should be Field3", false, false);
            testing.VerifyExpression(result.WorstMatchingField == "Field1", $"Worst matching field should be Field1. Expected: 'Field1', Actual: '{result.WorstMatchingField}'", true, "Worst matching field should be Field1", false, false);
        }
    }
}

