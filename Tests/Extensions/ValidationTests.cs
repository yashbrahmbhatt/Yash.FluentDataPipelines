using System;
using System.Text.RegularExpressions;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines.Tests.Extensions
{
    public class ValidationTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Validate_WithCustomValidator_Pass();
            Validate_WithCustomValidator_Fail();
            Validate_WithCustomErrorMessage();
            Validate_WithNullPipelineValue();
            Validate_WithExceptionInValidator();
            Before_DateIsBeforeTarget_Pass();
            Before_DateIsAfterTarget_Fail();
            Before_DateEqualsTarget_Fail();
            After_DateIsAfterTarget_Pass();
            After_DateIsBeforeTarget_Fail();
            After_DateEqualsTarget_Fail();
            GreaterThan_ValueGreater_Pass();
            GreaterThan_ValueEqual_Fail();
            GreaterThan_ValueLess_Fail();
            LessThan_ValueLess_Pass();
            LessThan_ValueEqual_Fail();
            LessThan_ValueGreater_Fail();
            Between_ValueInRangeInclusive_Pass();
            Between_ValueAtLowerBoundInclusive_Pass();
            Between_ValueAtUpperBoundInclusive_Pass();
            Between_ValueAtLowerBoundExclusive_Fail();
            Between_ValueAtUpperBoundExclusive_Fail();
            Between_ValueBelowRange_Fail();
            Between_ValueAboveRange_Fail();
            ApproximatelyEqual_WithinTolerance_Pass();
            ApproximatelyEqual_OutsideTolerance_Fail();
            ApproximatelyEqual_WithCustomTolerance();
            Contains_SubstringFound_Pass();
            Contains_SubstringNotFound_Fail();
            Contains_CaseSensitiveMatch();
            Contains_CaseInsensitiveMatch();
            Contains_NullValueHandling();
            Contains_NullSubstringHandling();
            Matches_PatternMatches_Pass();
            Matches_PatternDoesntMatch_Fail();
            Matches_CaseSensitiveRegex();
            Matches_CaseInsensitiveRegex();
            Matches_NullValueHandling();
            Matches_NullPatternHandling();
            MultipleValidations_AllPass();
            MultipleValidations_OneFails();
            MultipleValidations_MultipleFail();
            Validation_OnInvalidPipelineValue_PreservesState();
            ErrorAccumulation_AcrossValidations();
        }

        public void Validate_WithCustomValidator_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);
            var config = new ValidationConfig
            {
                CustomValidator = value => ((int)value) % 2 == 0,
                ErrorMessage = "Value must be even"
            };

            // Act
            var result = pipelineValue.Validate(config, (value, cfg) => cfg.CustomValidator(value));

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Validate_WithCustomValidator_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(41, true);
            var config = new ValidationConfig
            {
                CustomValidator = value => ((int)value) % 2 == 0,
                ErrorMessage = "Value must be even"
            };

            // Act
            var result = pipelineValue.Validate(config, (value, cfg) => cfg.CustomValidator(value));

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Errors.Count > 0, $"Should have error. Expected: > 0, Actual: {result.Errors.Count}", true, "Should have error", false, false);
        }


        public void Validate_WithCustomErrorMessage()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(41, true);
            var config = new ValidationConfig
            {
                CustomValidator = value => ((int)value) % 2 == 0,
                ErrorMessage = "Custom error message"
            };

            // Act
            var result = pipelineValue.Validate(config, (value, cfg) => cfg.CustomValidator(value));

            // Assert
            testing.VerifyExpression(result.Errors[0].Message == "Custom error message", $"Error message should match. Expected: 'Custom error message', Actual: '{result.Errors[0].Message}'", true, "Error message should match", false, false);
        }


        public void Validate_WithNullPipelineValue()
        {
            // Arrange
            PipelineValue<int> pipelineValue = null;
            var config = ValidationConfig.Default;

            // Act
            var result = pipelineValue.Validate(config, (value, cfg) => true);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Validate_WithExceptionInValidator()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);
            var config = ValidationConfig.Default;

            // Act
            var result = pipelineValue.Validate(config, (value, cfg) => throw new Exception("Validator error"));

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Errors.Count > 0, $"Should have error. Expected: > 0, Actual: {result.Errors.Count}", true, "Should have error", false, false);
        }


        public void Before_DateIsBeforeTarget_Pass()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-1);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.Before(DateTime.Now);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Before_DateIsAfterTarget_Fail()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.Before(DateTime.Now);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Before_DateEqualsTarget_Fail()
        {
            // Arrange
            var date = DateTime.Now;
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.Before(date);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void After_DateIsAfterTarget_Pass()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.After(DateTime.Now);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void After_DateIsBeforeTarget_Fail()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-1);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.After(DateTime.Now);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void After_DateEqualsTarget_Fail()
        {
            // Arrange
            var date = DateTime.Now;
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.After(date);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void GreaterThan_ValueGreater_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.GreaterThan(10);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void GreaterThan_ValueEqual_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.GreaterThan(42);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void GreaterThan_ValueLess_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(10, true);

            // Act
            var result = pipelineValue.GreaterThan(42);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void LessThan_ValueLess_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(10, true);

            // Act
            var result = pipelineValue.LessThan(42);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void LessThan_ValueEqual_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.LessThan(42);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void LessThan_ValueGreater_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.LessThan(10);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Between_ValueInRangeInclusive_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(30, true);
            var config = ValidationConfig.Default;

            // Act
            var result = pipelineValue.Between(20, 50, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Between_ValueAtLowerBoundInclusive_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(20, true);
            var config = new ValidationConfig { InclusiveLowerBound = true };

            // Act
            var result = pipelineValue.Between(20, 50, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Between_ValueAtUpperBoundInclusive_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(50, true);
            var config = new ValidationConfig { InclusiveUpperBound = true };

            // Act
            var result = pipelineValue.Between(20, 50, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Between_ValueAtLowerBoundExclusive_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(20, true);
            var config = new ValidationConfig { InclusiveLowerBound = false };

            // Act
            var result = pipelineValue.Between(20, 50, config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Between_ValueAtUpperBoundExclusive_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(50, true);
            var config = new ValidationConfig { InclusiveUpperBound = false };

            // Act
            var result = pipelineValue.Between(20, 50, config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Between_ValueBelowRange_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(10, true);

            // Act
            var result = pipelineValue.Between(20, 50);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Between_ValueAboveRange_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(60, true);

            // Act
            var result = pipelineValue.Between(20, 50);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void ApproximatelyEqual_WithinTolerance_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(1.0, true);
            var config = new ValidationConfig { Tolerance = 0.1 };

            // Act
            var result = pipelineValue.ApproximatelyEqual(1.05, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void ApproximatelyEqual_OutsideTolerance_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(1.0, true);
            var config = new ValidationConfig { Tolerance = 0.01 };

            // Act
            var result = pipelineValue.ApproximatelyEqual(1.05, config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void ApproximatelyEqual_WithCustomTolerance()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(1.0, true);
            var config = new ValidationConfig { Tolerance = 0.2 };

            // Act
            var result = pipelineValue.ApproximatelyEqual(1.15, config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Contains_SubstringFound_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("hello world", true);

            // Act
            var result = pipelineValue.Contains("world");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Contains_SubstringNotFound_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("hello world", true);

            // Act
            var result = pipelineValue.Contains("test");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Contains_CaseSensitiveMatch()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("Hello World", true);
            var config = new ValidationConfig { CaseSensitive = true };

            // Act
            var result = pipelineValue.Contains("hello", config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid (case sensitive). Expected: false, Actual: {result.IsValid}", true, "Should be invalid (case sensitive)", false, false);
        }


        public void Contains_CaseInsensitiveMatch()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("Hello World", true);
            var config = new ValidationConfig
            {
                CaseSensitive = false,
                StringComparison = StringComparison.OrdinalIgnoreCase
            };

            // Act
            var result = pipelineValue.Contains("hello", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid (case insensitive). Expected: true, Actual: {result.IsValid}", true, "Should be valid (case insensitive)", false, false);
        }


        public void Contains_NullValueHandling()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);

            // Act
            var result = pipelineValue.Contains("test");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Contains_NullSubstringHandling()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test", true);

            // Act
            var result = pipelineValue.Contains(null);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Matches_PatternMatches_Pass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("user@example.com", true);

            // Act
            var result = pipelineValue.Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Matches_PatternDoesntMatch_Fail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("invalid-email", true);

            // Act
            var result = pipelineValue.Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Matches_CaseSensitiveRegex()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("HELLO", true);
            var config = new ValidationConfig { CaseSensitive = true };

            // Act
            var result = pipelineValue.Matches("hello", config);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid (case sensitive). Expected: false, Actual: {result.IsValid}", true, "Should be invalid (case sensitive)", false, false);
        }


        public void Matches_CaseInsensitiveRegex()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("HELLO", true);
            var config = new ValidationConfig { CaseSensitive = false };

            // Act
            var result = pipelineValue.Matches("hello", config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid (case insensitive). Expected: true, Actual: {result.IsValid}", true, "Should be valid (case insensitive)", false, false);
        }


        public void Matches_NullValueHandling()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);

            // Act
            var result = pipelineValue.Matches(@"\w+");

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Matches_NullPatternHandling()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test", true);

            // Act
            var result = pipelineValue.Matches(null);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void MultipleValidations_AllPass()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(30, true);

            // Act
            var result = pipelineValue
                .GreaterThan(10)
                .LessThan(50)
                .Between(20, 40);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void MultipleValidations_OneFails()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(30, true);

            // Act
            var result = pipelineValue
                .GreaterThan(10)
                .LessThan(20)  // This will fail
                .Between(20, 40);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Errors.Count >= 1, $"Should have at least one error. Expected: >= 1, Actual: {result.Errors.Count}", true, "Should have at least one error", false, false);
        }


        public void MultipleValidations_MultipleFail()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(5, true);

            // Act
            var result = pipelineValue
                .GreaterThan(10)  // Fails
                .LessThan(3)       // Fails
                .Between(20, 40);  // Fails

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Errors.Count >= 1, $"Should have errors. Expected: >= 1, Actual: {result.Errors.Count}", true, "Should have errors", false, false);
        }


        public void Validation_OnInvalidPipelineValue_PreservesState()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(5, false, new[] { new PipelineError("Original error", "Op1") });

            // Act
            var result = pipelineValue.GreaterThan(10);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should remain invalid. Expected: false, Actual: {result.IsValid}", true, "Should remain invalid", false, false);
            testing.VerifyExpression(result.Errors.Count >= 1, $"Should have errors. Expected: >= 1, Actual: {result.Errors.Count}", true, "Should have errors", false, false);
        }


        public void ErrorAccumulation_AcrossValidations()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(5, true);

            // Act
            var result = pipelineValue
                .GreaterThan(10)
                .LessThan(3)
                .Between(20, 40);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Errors.Count >= 1, $"Should accumulate errors. Expected: >= 1, Actual: {result.Errors.Count}", true, "Should accumulate errors", false, false);
        }
    }
}

