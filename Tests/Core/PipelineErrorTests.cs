using System;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Tests.Core
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
            testing.VerifyExpression(error.Message == "Test error message", $"Message should be set. Expected: 'Test error message', Actual: '{error.Message}'", true, "Message should be set", false, false);
            testing.VerifyExpression(string.IsNullOrEmpty(error.Operation), $"Operation should be null. Expected: true, Actual: {string.IsNullOrEmpty(error.Operation)}", true, "Operation should be null", false, false);
            testing.VerifyExpression(error.Timestamp != default(DateTime), $"Timestamp should be set. Expected: not default(DateTime), Actual: {error.Timestamp}", true, "Timestamp should be set", false, false);
        }

        public void Constructor_WithMessageAndOperation_SetsBoth()
        {
            // Arrange & Act
            var error = new PipelineError("Test error message", "TestOperation");

            // Assert
            testing.VerifyExpression(error.Message == "Test error message", $"Message should be set. Expected: 'Test error message', Actual: '{error.Message}'", true, "Message should be set", false, false);
            testing.VerifyExpression(error.Operation == "TestOperation", $"Operation should be set. Expected: 'TestOperation', Actual: '{error.Operation}'", true, "Operation should be set", false, false);
            testing.VerifyExpression(error.Timestamp != default(DateTime), $"Timestamp should be set. Expected: not default(DateTime), Actual: {error.Timestamp}", true, "Timestamp should be set", false, false);
        }

        public void Constructor_WithNullMessage_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            try
            {
                var error = new PipelineError(null);
                testing.VerifyExpression(false, "Should have thrown ArgumentNullException. Expected: exception to be thrown, Actual: no exception", true, "Should have thrown ArgumentNullException", false, false);
            }
            catch (ArgumentNullException)
            {
                testing.VerifyExpression(true, "Correctly threw ArgumentNullException. Expected: exception thrown, Actual: exception thrown", true, "Correctly threw ArgumentNullException", false, false);
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
            testing.VerifyExpression(error.Timestamp >= beforeCreation, $"Timestamp should be after beforeCreation. Expected: >= {beforeCreation}, Actual: {error.Timestamp}", true, "Timestamp should be after beforeCreation", false, false);
            testing.VerifyExpression(error.Timestamp <= afterCreation, $"Timestamp should be before afterCreation. Expected: <= {afterCreation}, Actual: {error.Timestamp}", true, "Timestamp should be before afterCreation", false, false);
        }


        public void ToString_WithOperation_IncludesOperation()
        {
            // Arrange
            var error = new PipelineError("Test error message", "TestOperation");

            // Act
            var result = error.ToString();

            // Assert
            testing.VerifyExpression(result.Contains("Test error message"), $"Should contain message. Expected: result to contain 'Test error message', Actual: '{result}'", true, "Should contain message", false, false);
            testing.VerifyExpression(result.Contains("TestOperation"), $"Should contain operation. Expected: result to contain 'TestOperation', Actual: '{result}'", true, "Should contain operation", false, false);
            testing.VerifyExpression(result.Contains(":"), $"Should contain separator. Expected: result to contain ':', Actual: '{result}'", true, "Should contain separator", false, false);
        }


        public void ToString_WithoutOperation_ExcludesOperation()
        {
            // Arrange
            var error = new PipelineError("Test error message");

            // Act
            var result = error.ToString();

            // Assert
            testing.VerifyExpression(result.Contains("Test error message"), $"Should contain message. Expected: result to contain 'Test error message', Actual: '{result}'", true, "Should contain message", false, false);
            testing.VerifyExpression(!result.Contains(": "), $"Should not contain operation separator when no operation. Expected: result not to contain ': ', Actual: '{result}'", true, "Should not contain operation separator when no operation", false, false);
        }


        public void ToString_TimestampFormatting()
        {
            // Arrange
            var error = new PipelineError("Test error");

            // Act
            var result = error.ToString();

            // Assert
            // Should contain timestamp in format [yyyy-MM-dd HH:mm:ss]
            testing.VerifyExpression(result.Contains("["), $"Should contain opening bracket. Expected: result to contain '[', Actual: '{result}'", true, "Should contain opening bracket", false, false);
            testing.VerifyExpression(result.Contains("]"), $"Should contain closing bracket. Expected: result to contain ']', Actual: '{result}'", true, "Should contain closing bracket", false, false);
            testing.VerifyExpression(result.Length > 20, $"Should have reasonable length with timestamp. Expected: length > 20, Actual: {result.Length}", true, "Should have reasonable length with timestamp", false, false);
        }
    }
}

