using System;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines
{
    public class PipelineErrorTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Constructor_WithMessageOnly_SetsMessage();
            Constructor_WithMessageAndOperation_SetsBoth();
            Constructor_WithNullMessage_ThrowsArgumentNullException();
            Timestamp_IsSetAutomatically();
            ToString_WithOperation_IncludesOperation();
            ToString_WithoutOperation_ExcludesOperation();
            ToString_TimestampFormatting();
        }

        public void Constructor_WithMessageOnly_SetsMessage()
        {
            // Arrange & Act
            var error = new PipelineError("Test error message");

            // Assert
            testing.VerifyExpression(error.Message == "Test error message", "Message should be set");
            testing.VerifyExpression(string.IsNullOrEmpty(error.Operation), "Operation should be null");
            testing.VerifyExpression(error.Timestamp != default(DateTime), "Timestamp should be set");
        }

        public void Constructor_WithMessageAndOperation_SetsBoth()
        {
            // Arrange & Act
            var error = new PipelineError("Test error message", "TestOperation");

            // Assert
            testing.VerifyExpression(error.Message == "Test error message", "Message should be set");
            testing.VerifyExpression(error.Operation == "TestOperation", "Operation should be set");
            testing.VerifyExpression(error.Timestamp != default(DateTime), "Timestamp should be set");
        }

        public void Constructor_WithNullMessage_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            try
            {
                var error = new PipelineError(null);
                testing.VerifyExpression(false, "Should have thrown ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                testing.VerifyExpression(true, "Correctly threw ArgumentNullException");
            }
        }

        public void Timestamp_IsSetAutomatically()
        {
            // Arrange
            var beforeCreation = DateTime.Now;

            // Act
            var error = new PipelineError("Test error");
            var afterCreation = DateTime.Now;

            // Assert
            testing.VerifyExpression(error.Timestamp >= beforeCreation, "Timestamp should be after beforeCreation");
            testing.VerifyExpression(error.Timestamp <= afterCreation, "Timestamp should be before afterCreation");
        }


        public void ToString_WithOperation_IncludesOperation()
        {
            // Arrange
            var error = new PipelineError("Test error message", "TestOperation");

            // Act
            var result = error.ToString();

            // Assert
            testing.VerifyExpression(result.Contains("Test error message"), "Should contain message");
            testing.VerifyExpression(result.Contains("TestOperation"), "Should contain operation");
            testing.VerifyExpression(result.Contains(":"), "Should contain separator");
        }


        public void ToString_WithoutOperation_ExcludesOperation()
        {
            // Arrange
            var error = new PipelineError("Test error message");

            // Act
            var result = error.ToString();

            // Assert
            testing.VerifyExpression(result.Contains("Test error message"), "Should contain message");
            testing.VerifyExpression(!result.Contains(":"), "Should not contain separator when no operation");
        }


        public void ToString_TimestampFormatting()
        {
            // Arrange
            var error = new PipelineError("Test error");

            // Act
            var result = error.ToString();

            // Assert
            // Should contain timestamp in format [yyyy-MM-dd HH:mm:ss]
            testing.VerifyExpression(result.Contains("["), "Should contain opening bracket");
            testing.VerifyExpression(result.Contains("]"), "Should contain closing bracket");
            testing.VerifyExpression(result.Length > 20, "Should have reasonable length with timestamp");
        }
    }
}

