using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Tests.Core
{
    public class PipelineValueTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Constructor_WithValueOnly_DefaultsToValid();
            Constructor_WithValueAndIsValid_SetsCorrectly();
            Constructor_WithErrors_SetsErrorsCorrectly();
            Constructor_WithNullErrors_DefaultsToEmpty();
            Properties_AreReadOnly_Immutability();
            WithValue_TransformsValue_PreservesValidationState();
            WithValue_PreservesErrors();
            WithValidation_PassingValidation_RemainsValid();
            WithValidation_FailingValidation_BecomesInvalid();
            WithValidation_MultipleValidations_AllMustPass();
            WithValidation_OnInvalidValue_StaysInvalid();
            WithError_AddsSingleError();
            WithError_AddsMultipleErrors();
            WithError_SetsIsValidToFalse();
            WithError_ErrorsCollectionGrows();
            ImplicitConversion_PipelineValueToT();
            ImplicitConversion_TToPipelineValue();
            ImplicitConversion_NullHandling();
        }

        public void Constructor_WithValueOnly_DefaultsToValid()
        {
            // Arrange & Act
            var pipelineValue = new PipelineValue<string>("test");

            // Assert
            testing.VerifyExpression(pipelineValue.Value == "test", $"Value should be 'test'. Expected: 'test', Actual: '{pipelineValue.Value}'", true, "Value should be 'test'", false, false);
            testing.VerifyExpression(pipelineValue.IsValid == true, $"IsValid should default to true. Expected: true, Actual: {pipelineValue.IsValid}", true, "IsValid should default to true", false, false);
            testing.VerifyExpression(pipelineValue.Errors.Count == 0, $"Errors should be empty. Expected: 0, Actual: {pipelineValue.Errors.Count}", true, "Errors should be empty", false, false);
        }

        public void Constructor_WithValueAndIsValid_SetsCorrectly()
        {
            // Arrange & Act
            var validValue = new PipelineValue<string>("test", true);
            var invalidValue = new PipelineValue<string>("test", false);

            // Assert
            testing.VerifyExpression(validValue.IsValid == true, $"Valid value should have IsValid = true. Expected: true, Actual: {validValue.IsValid}", true, "Valid value should have IsValid = true", false, false);
            testing.VerifyExpression(invalidValue.IsValid == false, $"Invalid value should have IsValid = false. Expected: false, Actual: {invalidValue.IsValid}", true, "Invalid value should have IsValid = false", false, false);
        }

        public void Constructor_WithErrors_SetsErrorsCorrectly()
        {
            // Arrange
            var errors = new[] { new PipelineError("Error 1", "Op1"), new PipelineError("Error 2", "Op2") };

            // Act
            var pipelineValue = new PipelineValue<string>("test", false, errors);

            // Assert
            testing.VerifyExpression(pipelineValue.Errors.Count == 2, $"Should have 2 errors. Expected: 2, Actual: {pipelineValue.Errors.Count}", true, "Should have 2 errors", false, false);
            testing.VerifyExpression(pipelineValue.Errors[0].Message == "Error 1", $"First error message should match. Expected: 'Error 1', Actual: '{pipelineValue.Errors[0].Message}'", true, "First error message should match", false, false);
            testing.VerifyExpression(pipelineValue.Errors[1].Message == "Error 2", $"Second error message should match. Expected: 'Error 2', Actual: '{pipelineValue.Errors[1].Message}'", true, "Second error message should match", false, false);
        }

        public void Constructor_WithNullErrors_DefaultsToEmpty()
        {
            // Arrange & Act
            var pipelineValue = new PipelineValue<string>("test", true, null);

            // Assert
            testing.VerifyExpression(pipelineValue.Errors.Count == 0, $"Errors should default to empty when null. Expected: 0, Actual: {pipelineValue.Errors.Count}", true, "Errors should default to empty when null", false, false);
        }

        public void Properties_AreReadOnly_Immutability()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test");

            // Act & Assert
            // Properties should be read-only, so we can't modify them directly
            // This test verifies the structure is immutable
            testing.VerifyExpression(pipelineValue.Value == "test", $"Value should remain unchanged. Expected: 'test', Actual: '{pipelineValue.Value}'", true, "Value should remain unchanged", false, false);
            var newValue = pipelineValue.WithValue("new");
            testing.VerifyExpression(pipelineValue.Value == "test", $"Original value should remain unchanged. Expected: 'test', Actual: '{pipelineValue.Value}'", true, "Original value should remain unchanged", false, false);
            testing.VerifyExpression(newValue.Value == "new", $"New value should be different. Expected: 'new', Actual: '{newValue.Value}'", true, "New value should be different", false, false);
        }

        public void WithValue_TransformsValue_PreservesValidationState()
        {
            // Arrange
            var original = new PipelineValue<string>("test", true);

            // Act
            var transformed = original.WithValue<int>(42);

            // Assert
            testing.VerifyExpression(transformed.Value == 42, $"Value should be transformed. Expected: 42, Actual: {transformed.Value}", true, "Value should be transformed", false, false);
            testing.VerifyExpression(transformed.IsValid == true, $"Validation state should be preserved. Expected: true, Actual: {transformed.IsValid}", true, "Validation state should be preserved", false, false);
            testing.VerifyExpression(transformed.Errors.Count == 0, $"Errors should be preserved. Expected: 0, Actual: {transformed.Errors.Count}", true, "Errors should be preserved", false, false);
        }

        public void WithValue_PreservesErrors()
        {
            // Arrange
            var error = new PipelineError("Test error", "TestOp");
            var original = new PipelineValue<string>("test", false, new[] { error });

            // Act
            var transformed = original.WithValue<int>(42);

            // Assert
            testing.VerifyExpression(transformed.Errors.Count == 1, $"Errors should be preserved. Expected: 1, Actual: {transformed.Errors.Count}", true, "Errors should be preserved", false, false);
            testing.VerifyExpression(transformed.Errors[0].Message == "Test error", $"Error message should be preserved. Expected: 'Test error', Actual: '{transformed.Errors[0].Message}'", true, "Error message should be preserved", false, false);
            testing.VerifyExpression(transformed.IsValid == false, $"Invalid state should be preserved. Expected: false, Actual: {transformed.IsValid}", true, "Invalid state should be preserved", false, false);
        }

        public void WithValidation_PassingValidation_RemainsValid()
        {
            // Arrange
            var original = new PipelineValue<int>(42, true);

            // Act
            var result = original.WithValidation(true);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should remain valid. Expected: true, Actual: {result.IsValid}", true, "Should remain valid", false, false);
            testing.VerifyExpression(result.Errors.Count == 0, $"Should have no errors. Expected: 0, Actual: {result.Errors.Count}", true, "Should have no errors", false, false);
        }

        public void WithValidation_FailingValidation_BecomesInvalid()
        {
            // Arrange
            var original = new PipelineValue<int>(42, true);
            var error = new PipelineError("Validation failed", "Validate");

            // Act
            var result = original.WithValidation(false, error);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should become invalid. Expected: false, Actual: {result.IsValid}", true, "Should become invalid", false, false);
            testing.VerifyExpression(result.Errors.Count == 1, $"Should have one error. Expected: 1, Actual: {result.Errors.Count}", true, "Should have one error", false, false);
            testing.VerifyExpression(result.Errors[0].Message == "Validation failed", $"Error message should match. Expected: 'Validation failed', Actual: '{result.Errors[0].Message}'", true, "Error message should match", false, false);
        }

        public void WithValidation_MultipleValidations_AllMustPass()
        {
            // Arrange
            var original = new PipelineValue<int>(42, true);
            var error1 = new PipelineError("First validation failed", "Validate1");

            // Act
            var afterFirst = original.WithValidation(false, error1);
            var afterSecond = afterFirst.WithValidation(true);

            // Assert
            testing.VerifyExpression(afterSecond.IsValid == false, $"Should remain invalid if any validation failed. Expected: false, Actual: {afterSecond.IsValid}", true, "Should remain invalid if any validation failed", false, false);
            testing.VerifyExpression(afterSecond.Errors.Count == 1, $"Should have one error. Expected: 1, Actual: {afterSecond.Errors.Count}", true, "Should have one error", false, false);
        }

        public void WithValidation_OnInvalidValue_StaysInvalid()
        {
            // Arrange
            var original = new PipelineValue<int>(42, false, new[] { new PipelineError("Original error", "Op1") });

            // Act
            var result = original.WithValidation(true);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should stay invalid. Expected: false, Actual: {result.IsValid}", true, "Should stay invalid", false, false);
            testing.VerifyExpression(result.Errors.Count == 1, $"Original error should be preserved. Expected: 1, Actual: {result.Errors.Count}", true, "Original error should be preserved", false, false);
        }

        public void WithError_AddsSingleError()
        {
            // Arrange
            var original = new PipelineValue<string>("test", true);
            var error = new PipelineError("Test error", "TestOp");

            // Act
            var result = original.WithError(error);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should become invalid. Expected: false, Actual: {result.IsValid}", true, "Should become invalid", false, false);
            testing.VerifyExpression(result.Errors.Count == 1, $"Should have one error. Expected: 1, Actual: {result.Errors.Count}", true, "Should have one error", false, false);
            testing.VerifyExpression(result.Errors[0].Message == "Test error", $"Error message should match. Expected: 'Test error', Actual: '{result.Errors[0].Message}'", true, "Error message should match", false, false);
        }

        public void WithError_AddsMultipleErrors()
        {
            // Arrange
            var original = new PipelineValue<string>("test", true);
            var error1 = new PipelineError("Error 1", "Op1");
            var error2 = new PipelineError("Error 2", "Op2");

            // Act
            var afterFirst = original.WithError(error1);
            var afterSecond = afterFirst.WithError(error2);

            // Assert
            testing.VerifyExpression(afterSecond.Errors.Count == 2, $"Should have two errors. Expected: 2, Actual: {afterSecond.Errors.Count}", true, "Should have two errors", false, false);
            testing.VerifyExpression(afterSecond.Errors[0].Message == "Error 1", $"First error should match. Expected: 'Error 1', Actual: '{afterSecond.Errors[0].Message}'", true, "First error should match", false, false);
            testing.VerifyExpression(afterSecond.Errors[1].Message == "Error 2", $"Second error should match. Expected: 'Error 2', Actual: '{afterSecond.Errors[1].Message}'", true, "Second error should match", false, false);
        }

        public void WithError_SetsIsValidToFalse()
        {
            // Arrange
            var original = new PipelineValue<string>("test", true);

            // Act
            var result = original.WithError("Test error", "TestOp");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"IsValid should be false. Expected: false, Actual: {result.IsValid}", true, "IsValid should be false", false, false);
        }

        public void WithError_ErrorsCollectionGrows()
        {
            // Arrange
            var original = new PipelineValue<string>("test", true);

            // Act
            var result1 = original.WithError("Error 1", "Op1");
            var result2 = result1.WithError("Error 2", "Op2");
            var result3 = result2.WithError("Error 3", "Op3");

            // Assert
            testing.VerifyExpression(result3.Errors.Count == 3, $"Should have three errors. Expected: 3, Actual: {result3.Errors.Count}", true, "Should have three errors", false, false);
        }

        public void ImplicitConversion_PipelineValueToT()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test");

            // Act
            string value = pipelineValue;

            // Assert
            testing.VerifyExpression(value == "test", $"Implicit conversion should work. Expected: 'test', Actual: '{value}'", true, "Implicit conversion should work", false, false);
        }

        public void ImplicitConversion_TToPipelineValue()
        {
            // Arrange
            string value = "test";

            // Act
            PipelineValue<string> pipelineValue = value;

            // Assert
            testing.VerifyExpression(pipelineValue.Value == "test", $"Implicit conversion should work. Expected: 'test', Actual: '{pipelineValue.Value}'", true, "Implicit conversion should work", false, false);
            testing.VerifyExpression(pipelineValue.IsValid == true, $"Should default to valid. Expected: true, Actual: {pipelineValue.IsValid}", true, "Should default to valid", false, false);
        }

        public void ImplicitConversion_NullHandling()
        {
            // Arrange
            PipelineValue<string> nullPipelineValue = null;

            // Act
            string value = nullPipelineValue;

            // Assert
            testing.VerifyExpression(value == null, $"Null PipelineValue should convert to null. Expected: null, Actual: {value}", true, "Null PipelineValue should convert to null", false, false);
        }
    }
}

