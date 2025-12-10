using System;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines
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
            testing.VerifyExpression(result.Contains("Valid Date"), "Should format as valid");
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
            testing.VerifyExpression(result == "Invalid Date", "Should format as invalid");
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
            testing.VerifyExpression(result == "Valid: 42", "Should pass all validations");
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
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value > 100m, "Value should be > 100");
            testing.VerifyExpression(result.Value < 200m, "Value should be < 200");
        }


        public void ComplexChaining_WithMixedOperations()
        {
            // Arrange
            string source = "Product: Widget, Price: $99.99";

            // Act
            var result = source
                .Extract<decimal>(
                    new Configuration.ExtractConfig
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
            testing.VerifyExpression(result.Length > 0, "Should format currency");
            testing.VerifyExpression(result.Contains("$") || result.Contains("99"), "Should contain currency info");
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
            testing.VerifyExpression(result == "Failed", "Should fail");
            testing.VerifyExpression(pipelineValue.Errors.Count > 0, "Should have errors");
        }


        public void InvalidExtraction_FollowedByTransformations_NoOps()
        {
            // Arrange
            string source = "not-a-number";

            // Act
            var result = source
                .ExtractInt()
                .Multiply(2)
                .Divide(3)
                .Round(2);

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should remain invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have errors");
        }


        public void ExtractPrice_ValidateRange_AddTax_FormatCurrency()
        {
            // Arrange
            string source = "Product: Widget, Price: $99.99";

            // Act
            var result = source
                .Extract<decimal>(
                    new Configuration.ExtractConfig
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
            testing.VerifyExpression(result.Length > 0, "Should format currency");
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
            testing.VerifyExpression(result.Length > 0, "Should format date");
        }


        public void ExtractEmail_ValidateFormat_TransformToLowercase_Format()
        {
            // Arrange
            string source = "USER@EXAMPLE.COM";

            // Act
            var result = source
                .Contains("@")
                .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
                .ToLower()
                .Format((value, isValid) => isValid ? $"Valid email: {value}" : "Invalid email");

            // Assert
            testing.VerifyExpression(result.Contains("Valid email"), "Should be valid");
            testing.VerifyExpression(result.Contains("user@example.com"), "Should be lowercase");
        }


        public void ExtractNumbers_Filter_Transform_GetFirst()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            var result = numbers
                .Where(x => x > 5)
                .Select(x => x * 2)
                .First();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 12, "Should be first element (6 * 2)");
        }


        public void ComplexPipeline_WithMultipleValidations()
        {
            // Arrange
            string source = "user@example.com";

            // Act
            var result = source
                .Contains("@")
                .Contains(".")
                .Matches(@"^[\w\.-]+@[\w\.-]+\.\w+$")
                .Format((value, isValid) => isValid ? $"Valid email: {value}" : "Invalid email format");

            // Assert
            testing.VerifyExpression(result.Contains("Valid email"), "Should pass all validations");
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
            testing.VerifyExpression(extraction.IsValid == false, "Extraction should fail");
            testing.VerifyExpression(transformation.IsValid == false, "Transformation should preserve invalid state");
            testing.VerifyExpression(validation.IsValid == false, "Validation should preserve invalid state");
            testing.VerifyExpression(formatting == "Invalid", "Formatting should reflect invalid state");
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
            testing.VerifyExpression(result.Contains("Invalid"), "Should be invalid");
            testing.VerifyExpression(result.Contains("25"), "Should include value");
        }


        public void RealWorld_InvoiceProcessing()
        {
            // Arrange
            string invoiceText = "Invoice #12345, Amount: $1,234.56, Date: 2024-12-10";

            // Act - Extract amount
            var amount = invoiceText
                .Extract<decimal>(
                    new Configuration.ExtractConfig
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
            testing.VerifyExpression(amount.Length > 0, "Should format amount");
        }
    }
}

