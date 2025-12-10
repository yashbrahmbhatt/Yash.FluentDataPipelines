using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;
using Yash.FluentDataPipelines.Configuration;

namespace Yash.FluentDataPipelines.Tests.Integration
{
    public class IntegrationTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Extract_Transform_Validate_Format_AllPass();
            Extract_Transform_Validate_Format_ValidationFails();
            Extract_MultipleValidations_Format();
            Extract_MultipleTransformations_Validate();
            ComplexChaining_WithMixedOperations();
            ErrorAccumulation_ThroughPipeline();
            InvalidExtraction_FollowedByTransformations_NoOps();
            ExtractPrice_ValidateRange_AddTax_FormatCurrency();
            ExtractDate_ValidateDateRange_AddDays_Format();
            ExtractEmail_ValidateFormat_TransformToLowercase_Format();
            ExtractNumbers_Filter_Transform_GetFirst();
            ComplexPipeline_WithMultipleValidations();
            Pipeline_WithErrorRecovery();
            Pipeline_WithPartialFailure();
            RealWorld_InvoiceProcessing();
        }

        public void Extract_Transform_Validate_Format_AllPass()
        {
            // Arrange
            string source = "2024-12-10";

            // Act
            var result = source
                .ExtractDate()
                .AddDays(1)
                .Before(DateTime.Now.AddDays(7))
                .Format((value, isValid) => isValid ? $"Valid Date: {value:yyyy-MM-dd}" : "Invalid Date");

            // Assert
            testing.VerifyExpression(result.Contains("Valid Date"), $"Should format as valid. Expected: result to contain 'Valid Date', Actual: {result}", true, "Should format as valid", false, false);
        }


        public void Extract_Transform_Validate_Format_ValidationFails()
        {
            // Arrange
            string source = "2024-12-10";

            // Act
            var result = source
                .ExtractDate()
                .AddDays(1)
                .Before(DateTime.Now.AddDays(-7))  // This will fail
                .Format((value, isValid) => isValid ? $"Valid Date: {value:yyyy-MM-dd}" : "Invalid Date");

            // Assert
            testing.VerifyExpression(result == "Invalid Date", $"Should format as invalid. Expected: 'Invalid Date', Actual: '{result}'", true, "Invalid Date - Should format as invalid", false, false);
        }


        public void Extract_MultipleValidations_Format()
        {
            // Arrange
            string source = "42";

            // Act
            var result = source
                .ExtractInt()
                .GreaterThan(10)
                .LessThan(100)
                .Between(20, 50)
                .Format((value, isValid) => isValid ? $"Valid: {value}" : "Invalid");

            // Assert
            testing.VerifyExpression(result == "Valid: 42", $"Should pass all validations. Expected: 'Valid: 42', Actual: '{result}'", true, "Valid: 42 - Should pass all validations", false, false);
        }


        public void Extract_MultipleTransformations_Validate()
        {
            // Arrange
            string source = "99.99";

            // Act
            var result = source
                .ExtractDecimal()
                .Multiply(1.15m)
                .Round(2)
                .GreaterThan(100m)
                .LessThan(200m);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value > 100m, $"Value should be > 100. Expected: > 100, Actual: {result.Value}", true, "Value should be > 100", false, false);
            testing.VerifyExpression(result.Value < 200m, $"Value should be < 200. Expected: < 200, Actual: {result.Value}", true, "Value should be < 200", false, false);
        }


        public void ComplexChaining_WithMixedOperations()
        {
            // Arrange
            string source = "Product: Widget, Price: $99.99";

            // Act
            var result = source
                .Extract<decimal>(
                    new ExtractConfig
                    {
                        RegexPattern = @"Price: \$(\d+\.\d{2})",
                        GroupIndex = 1
                    },
                    (str, cfg) => decimal.Parse(str))
                .GreaterThan(50m)
                .LessThan(200m)
                .Multiply(1.15m)
                .Round(2)
                .FormatCurrency();

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format currency. Expected: length > 0, Actual: {result.Length}", true, "Should format currency", false, false);
            testing.VerifyExpression(result.Contains("$") || result.Contains("99"), $"Should contain currency info. Expected: result to contain '$' or '99', Actual: {result}", true, "Should contain currency info", false, false);
        }


        public void ErrorAccumulation_ThroughPipeline()
        {
            // Arrange
            string source = "invalid-date";

            // Act
            var pipelineValue = source
                .ExtractDate()
                .AddDays(1)
                .Before(DateTime.Now);
            var result = pipelineValue.Format((value, isValid) => isValid ? "OK" : "Failed");

            // Assert
            testing.VerifyExpression(result == "Failed", $"Should fail. Expected: 'Failed', Actual: '{result}'", true, "Failed - Should fail", false, false);
            testing.VerifyExpression(pipelineValue.Errors.Count > 0, $"Should have errors. Expected: > 0, Actual: {pipelineValue.Errors.Count}", true, "Should have errors", false, false);
        }


        public void InvalidExtraction_FollowedByTransformations_NoOps()
        {
            // Arrange
            string source = "not-a-number";

            // Act
            var result = source
                .ExtractInt()
                .Multiply(2)
                .Divide(3);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should remain invalid. Expected: false, Actual: {result.IsValid}", true, "Should remain invalid", false, false);
            testing.VerifyExpression(result.Errors.Count > 0, $"Should have errors. Expected: > 0, Actual: {result.Errors.Count}", true, "Should have errors", false, false);
        }


        public void ExtractPrice_ValidateRange_AddTax_FormatCurrency()
        {
            // Arrange
            string source = "Product: Widget, Price: $99.99";

            // Act
            var result = source
                .Extract<decimal>(
                    new Yash.FluentDataPipelines.Configuration.ExtractConfig
                    {
                        RegexPattern = @"Price: \$(\d+\.\d{2})",
                        GroupIndex = 1
                    },
                    (str, cfg) => decimal.Parse(str))
                .GreaterThan(50m)
                .LessThan(200m)
                .Multiply(1.15m)  // Add 15% tax
                .Round(2)
                .FormatCurrency();

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format currency. Expected: length > 0, Actual: {result.Length}", true, "Should format currency", false, false);
        }


        public void ExtractDate_ValidateDateRange_AddDays_Format()
        {
            // Arrange
            string source = "2024-12-10";

            // Act
            var result = source
                .ExtractDate()
                .After(DateTime.Now.AddDays(-30))
                .Before(DateTime.Now.AddDays(30))
                .AddDays(7)
                .Format("yyyy-MM-dd");

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format date. Expected: length > 0, Actual: {result.Length}", true, "Should format date", false, false);
        }


        public void ExtractEmail_ValidateFormat_TransformToLowercase_Format()
        {
            // Arrange
            string source = "USER@EXAMPLE.COM";

            // Act
            var result = ((PipelineValue<string>)source)
                .Contains("@")
                .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
                .ToLower()
                .Format((value, isValid) => isValid ? $"Valid email: {value}" : "Invalid email");

            // Assert
            testing.VerifyExpression(result.Contains("Valid email"), $"Should be valid. Expected: result to contain 'Valid email', Actual: {result}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Contains("user@example.com"), $"Should be lowercase. Expected: result to contain 'user@example.com', Actual: {result}", true, "Should be lowercase", false, false);
        }


        public void ExtractNumbers_Filter_Transform_GetFirst()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var result = ((PipelineValue<IEnumerable<int>>)numbers)
                .Where(x => x > 5)
                .Select(x => x * 2)
                .First();

            // Assert
            testing.VerifyExpression(result == 12, $"Should be first element (6 * 2). Expected: 12, Actual: {result}", true, "12 - Should be first element (6 * 2)", false, false);
        }


        public void ComplexPipeline_WithMultipleValidations()
        {
            // Arrange
            string source = "user@example.com";

            // Act
            var result = ((PipelineValue<string>)source)
                .Contains("@")
                .Contains(".")
                .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
                .Format((value, isValid) => isValid ? $"Valid email: {value}" : "Invalid email format");

            // Assert
            testing.VerifyExpression(result.Contains("Valid email"), $"Should pass all validations. Expected: result to contain 'Valid email', Actual: {result}", true, "Should pass all validations", false, false);
        }


        public void Pipeline_WithErrorRecovery()
        {
            // Arrange
            string source = "invalid";

            // Act
            var extraction = source.ExtractInt();
            var transformation = extraction.Multiply(2);
            var validation = transformation.GreaterThan(10);
            var formatting = validation.Format((value, isValid) => isValid ? $"Value: {value}" : "Invalid");

            // Assert
            testing.VerifyExpression(extraction.IsValid == false, $"Extraction should fail. Expected: false, Actual: {extraction.IsValid}", true, "Extraction should fail", false, false);
            testing.VerifyExpression(transformation.IsValid == false, $"Transformation should preserve invalid state. Expected: false, Actual: {transformation.IsValid}", true, "Transformation should preserve invalid state", false, false);
            testing.VerifyExpression(validation.IsValid == false, $"Validation should preserve invalid state. Expected: false, Actual: {validation.IsValid}", true, "Validation should preserve invalid state", false, false);
            testing.VerifyExpression(formatting == "Invalid", $"Formatting should reflect invalid state. Expected: 'Invalid', Actual: '{formatting}'", true, "Invalid - Formatting should reflect invalid state", false, false);
        }


        public void Pipeline_WithPartialFailure()
        {
            // Arrange
            string source = "25";

            // Act
            var result = source
                .ExtractInt()
                .GreaterThan(10)  // Pass
                .LessThan(20)     // Fail
                .Between(30, 50)  // Fail
                .Format((value, isValid) => isValid ? $"Valid: {value}" : $"Invalid: {value}");

            // Assert
            testing.VerifyExpression(result.Contains("Invalid"), $"Should be invalid. Expected: result to contain 'Invalid', Actual: {result}", true, "Should be invalid", false, false);
            testing.VerifyExpression(result.Contains("25"), $"Should include value. Expected: result to contain '25', Actual: {result}", true, "Should include value", false, false);
        }


        public void RealWorld_InvoiceProcessing()
        {
            // Arrange
            string invoiceText = "Invoice #12345, Amount: $1,234.56, Date: 2024-12-10";

            // Act - Extract amount
            var amount = invoiceText
                .Extract<decimal>(
                    new Yash.FluentDataPipelines.Configuration.ExtractConfig
                    {
                        RegexPattern = @"Amount: \$([\d,]+\.\d{2})",
                        GroupIndex = 1,
                        NumberStyles = System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint
                    },
                    (str, cfg) => decimal.Parse(str.Replace(",", ""), cfg.NumberStyles, cfg.Culture))
                .GreaterThan(0m)
                .LessThan(10000m)
                .FormatCurrency();

            // Assert
            testing.VerifyExpression(amount.Length > 0, $"Should format amount. Expected: length > 0, Actual: {amount.Length}", true, "Should format amount", false, false);
        }
    }
}

