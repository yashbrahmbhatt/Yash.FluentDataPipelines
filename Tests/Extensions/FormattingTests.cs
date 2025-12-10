using System;
using System.Globalization;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines
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
            testing.VerifyExpression(result == "42", "Should format to string");
        }


        public void Format_WithFormatString()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.Format("yyyy-MM-dd");

            // Assert
            testing.VerifyExpression(result == "2024-12-10", "Should format with format string");
        }


        public void Format_WithCustomFormatter()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, true);

            // Act
            var result = pipelineValue.Format(value => $"Count: {value}");

            // Assert
            testing.VerifyExpression(result == "Count: 42", "Should use custom formatter");
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
            testing.VerifyExpression(validResult == "Valid: 42", "Valid value should format correctly");
            testing.VerifyExpression(invalidResult == "Invalid", "Invalid value should format correctly");
        }


        public void Format_NullValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);
            var config = new FormatConfig { NullValueString = "NULL" };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "NULL", "Should use NullValueString");
        }


        public void Format_InvalidValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(42, false);
            var config = new FormatConfig { InvalidValueString = "INVALID" };

            // Act
            var result = pipelineValue.Format(config);

            // Assert
            testing.VerifyExpression(result == "INVALID", "Should use InvalidValueString");
        }


        public void Format_WithNullPipelineValue()
        {
            // Arrange
            PipelineValue<int> pipelineValue = null;

            // Act
            var result = pipelineValue.Format();

            // Assert
            testing.VerifyExpression(result == "null", "Should return 'null' string");
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
            testing.VerifyExpression(result == "2024-12-10", "Should format IFormattable type");
        }


        public void FormatDate_WithDefaultFormat()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.FormatDate();

            // Assert
            testing.VerifyExpression(result.Length > 0, "Should format date");
        }


        public void FormatDate_WithCustomFormatString()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.FormatDate("yyyy-MM-dd");

            // Assert
            testing.VerifyExpression(result == "2024-12-10", "Should format with custom format");
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
            testing.VerifyExpression(result.Length > 0, "Should still format date");
        }


        public void FormatNumber_WithDefaultFormat()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.999m, true);

            // Act
            var result = pipelineValue.FormatNumber();

            // Assert
            testing.VerifyExpression(result.Contains("99"), "Should format number");
            testing.VerifyExpression(result.Contains("."), "Should include decimal");
        }


        public void FormatNumber_WithCustomFormat()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.999m, true);

            // Act
            var result = pipelineValue.FormatNumber("F0");

            // Assert
            testing.VerifyExpression(result == "100", "Should format with custom format");
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
            testing.VerifyExpression(intResult.Contains("42"), "Int should format");
            testing.VerifyExpression(doubleResult.Contains("42"), "Double should format");
            testing.VerifyExpression(decimalResult.Contains("42"), "Decimal should format");
        }


        public void FormatCurrency_WithDefaultCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.99m, true);

            // Act
            var result = pipelineValue.FormatCurrency();

            // Assert
            testing.VerifyExpression(result.Length > 0, "Should format currency");
            testing.VerifyExpression(result.Contains("99"), "Should contain value");
        }


        public void FormatCurrency_WithCustomCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.99m, true);
            var culture = CultureInfo.GetCultureInfo("en-US");

            // Act
            var result = pipelineValue.FormatCurrency(culture);

            // Assert
            testing.VerifyExpression(result.Length > 0, "Should format currency");
            testing.VerifyExpression(result.Contains("$") || result.Contains("99"), "Should contain currency symbol or value");
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
            testing.VerifyExpression(intResult.Length > 0, "Int should format");
            testing.VerifyExpression(doubleResult.Length > 0, "Double should format");
            testing.VerifyExpression(decimalResult.Length > 0, "Decimal should format");
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
            testing.VerifyExpression(result == "Custom: 42", "Custom formatter should take precedence");
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
            testing.VerifyExpression(result.Contains("Validation"), "CustomFormatterWithValidation should take precedence");
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
            testing.VerifyExpression(result == "Invalid: 42", "Should use CustomFormatterWithValidation for invalid");
        }
    }
}

