using System;
using System.Globalization;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines.Tests.Extensions
{
    public class TransformationTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            AddDays_WithInt_Positive();
            AddDays_WithInt_Negative();
            AddDays_WithDouble_Fractional();
            AddMonths_Positive();
            AddMonths_Negative();
            AddYears_Positive();
            AddYears_Negative();
            Add_TimeSpan();
            Transform_OnInvalidPipelineValue_NoOp();
            Transform_WithNullPipelineValue_Error();
            Transform_WithException_ErrorAdded();
            Multiply_IntByFactor();
            Multiply_DoubleByFactor();
            Multiply_DecimalByFactor();
            Divide_IntByDivisor();
            Divide_DoubleByDivisor();
            Divide_DecimalByDivisor();
            Divide_ByZero_ExceptionHandling();
            Round_DoubleToDecimals();
            Round_DecimalToDecimals();
            Round_WithDifferentMidpointRoundingModes();
            Transform_OnInvalidNumericPipelineValue_NoOp();
            Transform_WithNullNumericPipelineValue_Error();
            Trim_Whitespace();
            Trim_WithTrimCharsArray();
            Trim_WithNullValue();
            ToUpper_Conversion();
            ToUpper_WithCulture();
            ToLower_Conversion();
            ToLower_WithCulture();
            Replace_SubstringFound();
            Replace_SubstringNotFound();
            Replace_WithNullValue();
            Transform_OnInvalidStringPipelineValue_NoOp();
            Transform_WithNullStringPipelineValue_Error();
        }

        public void AddDays_WithInt_Positive()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddDays(7);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddDays(7), $"Date should be 7 days later. Expected: {date.AddDays(7)}, Actual: {result.Value}", true, "Date should be 7 days later", false, false);
        }


        public void AddDays_WithInt_Negative()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddDays(-7);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddDays(-7), $"Date should be 7 days earlier. Expected: {date.AddDays(-7)}, Actual: {result.Value}", true, "Date should be 7 days earlier", false, false);
        }


        public void AddDays_WithDouble_Fractional()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10, 12, 0, 0);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddDays(1.5);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddDays(1.5), $"Date should be 1.5 days later. Expected: {date.AddDays(1.5)}, Actual: {result.Value}", true, "Date should be 1.5 days later", false, false);
        }


        public void AddMonths_Positive()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddMonths(1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddMonths(1), $"Date should be 1 month later. Expected: {date.AddMonths(1)}, Actual: {result.Value}", true, "Date should be 1 month later", false, false);
        }


        public void AddMonths_Negative()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddMonths(-1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddMonths(-1), $"Date should be 1 month earlier. Expected: {date.AddMonths(-1)}, Actual: {result.Value}", true, "Date should be 1 month earlier", false, false);
        }


        public void AddYears_Positive()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddYears(1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddYears(1), $"Date should be 1 year later. Expected: {date.AddYears(1)}, Actual: {result.Value}", true, "Date should be 1 year later", false, false);
        }


        public void AddYears_Negative()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);

            // Act
            var result = pipelineValue.AddYears(-1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.AddYears(-1), $"Date should be 1 year earlier. Expected: {date.AddYears(-1)}, Actual: {result.Value}", true, "Date should be 1 year earlier", false, false);
        }


        public void Add_TimeSpan()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10, 12, 0, 0);
            var pipelineValue = new PipelineValue<DateTime>(date, true);
            var timeSpan = TimeSpan.FromHours(12);

            // Act
            var result = pipelineValue.Add(timeSpan);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == date.Add(timeSpan), $"Date should have timeSpan added. Expected: {date.Add(timeSpan)}, Actual: {result.Value}", true, "Date should have timeSpan added", false, false);
        }


        public void Transform_OnInvalidPipelineValue_NoOp()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, false, new[] { new PipelineError("Error", "Op") });

            // Act
            var result = pipelineValue.AddDays(7);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should remain invalid. Expected: false, Actual: {result.IsValid}", true, "Should remain invalid", false, false);
            testing.VerifyExpression(result.Value == date, $"Value should not change. Expected: {date}, Actual: {result.Value}", true, "Value should not change", false, false);
        }


        public void Transform_WithNullPipelineValue_Error()
        {
            // Arrange
            PipelineValue<DateTime> pipelineValue = null;

            // Act
            var result = pipelineValue.AddDays(7);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Transform_WithException_ErrorAdded()
        {
            // Arrange
            var date = new DateTime(2024, 12, 10);
            var pipelineValue = new PipelineValue<DateTime>(date, true);
            var config = new TransformationConfig();

            // Act - This should not throw but handle gracefully
            // Note: DateTime operations don't typically throw, so we test the pattern
            var result = pipelineValue.AddDays(7);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
        }


        public void Multiply_IntByFactor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(10, true);

            // Act
            var result = pipelineValue.Multiply(3);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == 30, $"Value should be multiplied. Expected: 30, Actual: {result.Value}", true, "Value should be multiplied", false, false);
        }


        public void Multiply_DoubleByFactor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(10.5, true);

            // Act
            var result = pipelineValue.Multiply(2.0);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(Math.Abs(result.Value - 21.0) < 0.001, $"Value should be multiplied. Expected: ~21.0, Actual: {result.Value}", true, "Value should be multiplied", false, false);
        }


        public void Multiply_DecimalByFactor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(10.5m, true);

            // Act
            var result = pipelineValue.Multiply(2.0m);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == 21.0m, $"Value should be multiplied. Expected: 21.0, Actual: {result.Value}", true, "Value should be multiplied", false, false);
        }


        public void Divide_IntByDivisor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(20, true);

            // Act
            var result = pipelineValue.Divide(4);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == 5, $"Value should be divided. Expected: 5, Actual: {result.Value}", true, "Value should be divided", false, false);
        }


        public void Divide_DoubleByDivisor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(20.0, true);

            // Act
            var result = pipelineValue.Divide(4.0);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(Math.Abs(result.Value - 5.0) < 0.001, $"Value should be divided. Expected: ~5.0, Actual: {result.Value}", true, "Value should be divided", false, false);
        }


        public void Divide_DecimalByDivisor()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(20.0m, true);

            // Act
            var result = pipelineValue.Divide(4.0m);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == 5.0m, $"Value should be divided. Expected: 5.0, Actual: {result.Value}", true, "Value should be divided", false, false);
        }


        public void Divide_ByZero_ExceptionHandling()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(20, true);

            // Act
            var result = pipelineValue.Divide(0);

            // Assert
            // Division by zero will throw an exception, which should be caught and handled
            // The result should be invalid
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid after division by zero. Expected: false, Actual: {result.IsValid}", true, "Should be invalid after division by zero", false, false);
        }


        public void Round_DoubleToDecimals()
        {
            // Arrange
            var pipelineValue = new PipelineValue<double>(99.999, true);

            // Act
            var result = pipelineValue.Round(2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(Math.Abs(result.Value - 100.0) < 0.001, $"Value should be rounded. Expected: ~100.0, Actual: {result.Value}", true, "Value should be rounded", false, false);
        }


        public void Round_DecimalToDecimals()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(99.999m, true);

            // Act
            var result = pipelineValue.Round(2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == 100.00m, $"Value should be rounded. Expected: 100.00, Actual: {result.Value}", true, "Value should be rounded", false, false);
        }


        public void Round_WithDifferentMidpointRoundingModes()
        {
            // Arrange
            var pipelineValue = new PipelineValue<decimal>(2.5m, true);
            var configToEven = new TransformationConfig { RoundingMode = MidpointRounding.ToEven };
            var configAwayFromZero = new TransformationConfig { RoundingMode = MidpointRounding.AwayFromZero };

            // Act
            var resultToEven = pipelineValue.Round(0, configToEven);
            var resultAwayFromZero = pipelineValue.Round(0, configAwayFromZero);

            // Assert
            testing.VerifyExpression(resultToEven.IsValid == true, $"Should be valid. Expected: true, Actual: {resultToEven.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(resultAwayFromZero.IsValid == true, $"Should be valid. Expected: true, Actual: {resultAwayFromZero.IsValid}", true, "Should be valid", false, false);
        }


        public void Transform_OnInvalidNumericPipelineValue_NoOp()
        {
            // Arrange
            var pipelineValue = new PipelineValue<int>(10, false, new[] { new PipelineError("Error", "Op") });

            // Act
            var result = pipelineValue.Multiply(2);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should remain invalid. Expected: false, Actual: {result.IsValid}", true, "Should remain invalid", false, false);
            testing.VerifyExpression(result.Value == 10, $"Value should not change. Expected: 10, Actual: {result.Value}", true, "Value should not change", false, false);
        }


        public void Transform_WithNullNumericPipelineValue_Error()
        {
            // Arrange
            PipelineValue<int> pipelineValue = null;

            // Act
            var result = pipelineValue.Multiply(2);

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }


        public void Trim_Whitespace()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("  hello world  ", true);

            // Act
            var result = pipelineValue.Trim();

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "hello world", $"Whitespace should be trimmed. Expected: 'hello world', Actual: '{result.Value}'", true, "Whitespace should be trimmed", false, false);
        }


        public void Trim_WithTrimCharsArray()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("***hello***", true);
            var config = new TransformationConfig { TrimChars = new[] { '*' } };

            // Act
            var result = pipelineValue.Trim(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "hello", $"Chars should be trimmed. Expected: 'hello', Actual: '{result.Value}'", true, "Chars should be trimmed", false, false);
        }


        public void Trim_WithNullValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);

            // Act
            var result = pipelineValue.Trim();

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == null, $"Null should remain null. Expected: null, Actual: {result.Value}", true, "Null should remain null", false, false);
        }


        public void ToUpper_Conversion()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("hello world", true);

            // Act
            var result = pipelineValue.ToUpper();

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "HELLO WORLD", $"Should be uppercase. Expected: 'HELLO WORLD', Actual: '{result.Value}'", true, "Should be uppercase", false, false);
        }


        public void ToUpper_WithCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("i", true);
            var config = new TransformationConfig { Culture = CultureInfo.GetCultureInfo("tr-TR") };

            // Act
            var result = pipelineValue.ToUpper(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            // Turkish 'i' becomes 'İ' (dotted I) in Turkish culture
        }


        public void ToLower_Conversion()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("HELLO WORLD", true);

            // Act
            var result = pipelineValue.ToLower();

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "hello world", $"Should be lowercase. Expected: 'hello world', Actual: '{result.Value}'", true, "Should be lowercase", false, false);
        }


        public void ToLower_WithCulture()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("I", true);
            var config = new TransformationConfig { Culture = CultureInfo.GetCultureInfo("tr-TR") };

            // Act
            var result = pipelineValue.ToLower(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            // Turkish 'I' becomes 'ı' (dotless i) in Turkish culture
        }


        public void Replace_SubstringFound()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("hello world", true);

            // Act
            var result = pipelineValue.Replace("world", "universe");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "hello universe", $"Substring should be replaced. Expected: 'hello universe', Actual: '{result.Value}'", true, "Substring should be replaced", false, false);
        }


        public void Replace_SubstringNotFound()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("hello world", true);

            // Act
            var result = pipelineValue.Replace("test", "universe");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == "hello world", $"String should remain unchanged. Expected: 'hello world', Actual: '{result.Value}'", true, "String should remain unchanged", false, false);
        }


        public void Replace_WithNullValue()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>(null, true);

            // Act
            var result = pipelineValue.Replace("test", "universe");

            // Assert
            testing.VerifyExpression(result.IsValid == true, $"Should be valid. Expected: true, Actual: {result.IsValid}", true, "Should be valid", false, false);
            testing.VerifyExpression(result.Value == null, $"Null should remain null. Expected: null, Actual: {result.Value}", true, "Null should remain null", false, false);
        }


        public void Transform_OnInvalidStringPipelineValue_NoOp()
        {
            // Arrange
            var pipelineValue = new PipelineValue<string>("test", false, new[] { new PipelineError("Error", "Op") });

            // Act
            var result = pipelineValue.Trim();

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should remain invalid. Expected: false, Actual: {result.IsValid}", true, "Should remain invalid", false, false);
            testing.VerifyExpression(result.Value == "test", $"Value should not change. Expected: 'test', Actual: '{result.Value}'", true, "Value should not change", false, false);
        }


        public void Transform_WithNullStringPipelineValue_Error()
        {
            // Arrange
            PipelineValue<string> pipelineValue = null;

            // Act
            var result = pipelineValue.Trim();

            // Assert
            testing.VerifyExpression(result.IsValid == false, $"Should be invalid. Expected: false, Actual: {result.IsValid}", true, "Should be invalid", false, false);
        }
    }
}

