using System;
using System.Globalization;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines.Tests.Extensions
{
    public class FormattingTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Format_WithDefaultConfig();
            Format_WithFormatString();
            Format_WithCustomFormatter();
            Format_WithCustomFormatterWithValidation();
            Format_NullValue();
            Format_InvalidValue();
            Format_WithNullPipelineValue();
            Format_WithIFormattableType();
            FormatDate_WithDefaultFormat();
            FormatDate_WithCustomFormatString();
            FormatDate_WithInvalidDate();
            FormatNumber_WithDefaultFormat();
            FormatNumber_WithCustomFormat();
            FormatNumber_WithDifferentNumericTypes();
            FormatCurrency_WithDefaultCulture();
            FormatCurrency_WithCustomCulture();
            FormatCurrency_WithDifferentNumericTypes();
            Format_CustomFormatterTakesPrecedence();
            Format_CustomFormatterWithValidationTakesPrecedence();
            Format_InvalidValueUsesCustomFormatterWithValidation();
        }

        public void Format_WithDefaultConfig()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.Format();

            // Assert
            testing.VerifyExpression(result == "42", $"Should format to string. Expected: '42', Actual: '{result}'", true, "Should format to string", false, false);
        }


        public void Format_WithFormatString()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.Format("yyyy-MM-dd");

            // Assert
            testing.VerifyExpression(result == "2024-12-10", $"Should format with format string. Expected: '2024-12-10', Actual: '{result}'", true, "Should format with format string", false, false);
        }


        public void Format_WithCustomFormatter()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.Format(value => $"Count: {value}");

            // Assert
            testing.VerifyExpression(result == "Count: 42", $"Should use custom formatter. Expected: 'Count: 42', Actual: '{result}'", true, "Should use custom formatter", false, false);
        }


        public void Format_WithCustomFormatterWithValidation()
        {
            // Arrange
            var validValue = new PipelineValue<int>(42, true);
            var invalidValue = new PipelineValue<int>(42, false);

            // Act
            var validResult = validValue.Format((value, isValid) => isValid ? $"Valid: {value}" : "Invalid");
            var invalidResult = invalidValue.Format((value, isValid) => isValid ? $"Valid: {value}" : "Invalid");

            // Assert
            testing.VerifyExpression(validResult == "Valid: 42", $"Valid value should format correctly. Expected: 'Valid: 42', Actual: '{validResult}'", true, "Valid value should format correctly", false, false);
            testing.VerifyExpression(invalidResult == "Invalid", $"Invalid value should format correctly. Expected: 'Invalid', Actual: '{invalidResult}'", true, "Invalid value should format correctly", false, false);
        }


        public void Format_NullValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);
            var config = new FormatConfig { NullValueString = "NULL" };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "NULL", $"Should use NullValueString. Expected: 'NULL', Actual: '{result}'", true, "Should use NullValueString", false, false);
        }


        public void Format_InvalidValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, false);
            var config = new FormatConfig { InvalidValueString = "INVALID" };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "INVALID", $"Should use InvalidValueString. Expected: 'INVALID', Actual: '{result}'", true, "Should use InvalidValueString", false, false);
        }


        public void Format_WithNullPipelineValue()
        {
            // Arrange
            PipelineValue<int> pipelineValue = null;

            // Act
            var result = pipelineValue.Format();

            // Assert
            testing.VerifyExpression(result == "null", $"Should return 'null' string. Expected: 'null', Actual: '{result}'", true, "Should return 'null' string", false, false);
        }


        public void Format_WithIFormattableType()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);
            var config = new FormatConfig { FormatString = "yyyy-MM-dd" };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "2024-12-10", $"Should format IFormattable type. Expected: '2024-12-10', Actual: '{result}'", true, "Should format IFormattable type", false, false);
        }


        public void FormatDate_WithDefaultFormat()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.FormatDate();

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format date. Expected: length > 0, Actual: {result.Length}", true, "Should format date", false, false);
        }


        public void FormatDate_WithCustomFormatString()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.FormatDate("yyyy-MM-dd");

            // Assert
            testing.VerifyExpression(result == "2024-12-10", $"Should format with custom format. Expected: '2024-12-10', Actual: '{result}'", true, "Should format with custom format", false, false);
        }


        public void FormatDate_WithInvalidDate()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, false);
            var config = new FormatConfig { InvalidValueString = "INVALID" };

            // Act
            var result = pipelineValue.FormatDate("yyyy-MM-dd");

            // Assert
            // Note: FormatDate doesn't use config, so it will format the date even if invalid
            // This tests the behavior
            testing.VerifyExpression(result.Length > 0, $"Should still format date. Expected: length > 0, Actual: {result.Length}", true, "Should still format date", false, false);
        }


        public void FormatNumber_WithDefaultFormat()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.999m, true);

            // Act
            var result = pipelineValue.FormatNumber();

            // Assert
            testing.VerifyExpression(result.Contains("100"), $"Should format number. Expected: result to contain '100', Actual: '{result}'", true, "Should format number", false, false);
            testing.VerifyExpression(result.Contains("."), $"Should include decimal. Expected: result to contain '.', Actual: '{result}'", true, "Should include decimal", false, false);
        }


        public void FormatNumber_WithCustomFormat()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.999m, true);

            // Act
            var result = pipelineValue.FormatNumber("F0");

            // Assert
            testing.VerifyExpression(result == "100", $"Should format with custom format. Expected: '100', Actual: '{result}'", true, "Should format with custom format", false, false);
        }


        public void FormatNumber_WithDifferentNumericTypes()
        {
            // Arrange
            var intValue = new PipelineValue<int>(42, true);
            var doubleValue = new PipelineValue<double>(42.5, true);
            var decimalValue = new PipelineValue<decimal>(42.5m, true);

            // Act
            var intResult = intValue.FormatNumber("N0");
            var doubleResult = doubleValue.FormatNumber("N1");
            var decimalResult = decimalValue.FormatNumber("N1");

            // Assert
            testing.VerifyExpression(intResult.Contains("42"), $"Int should format. Expected: result to contain '42', Actual: '{intResult}'", true, "Int should format", false, false);
            testing.VerifyExpression(doubleResult.Contains("42"), $"Double should format. Expected: result to contain '42', Actual: '{doubleResult}'", true, "Double should format", false, false);
            testing.VerifyExpression(decimalResult.Contains("42"), $"Decimal should format. Expected: result to contain '42', Actual: '{decimalResult}'", true, "Decimal should format", false, false);
        }


        public void FormatCurrency_WithDefaultCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.99m, true);

            // Act
            var result = pipelineValue.FormatCurrency();

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format currency. Expected: length > 0, Actual: {result.Length}", true, "Should format currency", false, false);
            testing.VerifyExpression(result.Contains("99"), $"Should contain value. Expected: result to contain '99', Actual: '{result}'", true, "Should contain value", false, false);
        }


        public void FormatCurrency_WithCustomCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.99m, true);
            var culture = CultureInfo.GetCultureInfo("en-US");

            // Act
            var result = pipelineValue.FormatCurrency(culture);

            // Assert
            testing.VerifyExpression(result.Length > 0, $"Should format currency. Expected: length > 0, Actual: {result.Length}", true, "Should format currency", false, false);
            testing.VerifyExpression(result.Contains("$") || result.Contains("99"), $"Should contain currency symbol or value. Expected: result to contain '$' or '99', Actual: {result}", true, "Should contain currency symbol or value", false, false);
        }


        public void FormatCurrency_WithDifferentNumericTypes()
        {
            // Arrange
            var intValue = new PipelineValue<int>(99, true);
            var doubleValue = new PipelineValue<double>(99.99, true);
            var decimalValue = new PipelineValue<decimal>(99.99m, true);

            // Act
            var intResult = intValue.FormatCurrency();
            var doubleResult = doubleValue.FormatCurrency();
            var decimalResult = decimalValue.FormatCurrency();

            // Assert
            testing.VerifyExpression(intResult.Length > 0, $"Int should format. Expected: length > 0, Actual: {intResult.Length}", true, "Int should format", false, false);
            testing.VerifyExpression(doubleResult.Length > 0, $"Double should format. Expected: length > 0, Actual: {doubleResult.Length}", true, "Double should format", false, false);
            testing.VerifyExpression(decimalResult.Length > 0, $"Decimal should format. Expected: length > 0, Actual: {decimalResult.Length}", true, "Decimal should format", false, false);
        }


        public void Format_CustomFormatterTakesPrecedence()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);
            var config = new FormatConfig
            {
                FormatString = "N2",
                CustomFormatter = value => "Custom: " + value
            };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "Custom: 42", $"Custom formatter should take precedence. Expected: 'Custom: 42', Actual: '{result}'", true, "Custom formatter should take precedence", false, false);
        }


        public void Format_CustomFormatterWithValidationTakesPrecedence()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);
            var config = new FormatConfig
            {
                FormatString = "N2",
                CustomFormatter = value => "Custom: " + value,
                CustomFormatterWithValidation = (value, isValid) => $"Validation: {isValid} - {value}"
            };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result.Contains("Validation"), $"CustomFormatterWithValidation should take precedence. Expected: result to contain 'Validation', Actual: '{result}'", true, "CustomFormatterWithValidation should take precedence", false, false);
        }


        public void Format_InvalidValueUsesCustomFormatterWithValidation()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, false);
            var config = new FormatConfig
            {
                CustomFormatterWithValidation = (value, isValid) => isValid ? $"Valid: {value}" : $"Invalid: {value}"
            };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "Invalid: 42", $"Should use CustomFormatterWithValidation for invalid. Expected: 'Invalid: 42', Actual: '{result}'", true, "Should use CustomFormatterWithValidation for invalid", false, false);
        }
    }
}

