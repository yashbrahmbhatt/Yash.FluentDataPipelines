using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines
{
    public class StringExtractionTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Extract_WithNullSource_ReturnsInvalid();
            Extract_WithEmptySource_ReturnsInvalid();
            Extract_WithRegexPattern_Success();
            Extract_WithRegexPattern_NoMatch();
            Extract_WithRegexGroupIndex_Valid();
            Extract_WithRegexGroupIndex_OutOfRange();
            Extract_WithRegexOptions_IgnoreCase();
            Extract_WithThrowOnFailure_True_ThrowsException();
            Extract_WithThrowOnFailure_False_ReturnsInvalid();
            Extract_WithExceptionInConverter_HandledGracefully();
            ExtractDate_ValidDateString_DefaultFormat();
            ExtractDate_WithCustomDateTimeFormat();
            ExtractDate_WithDateTimeFormatsArray();
            ExtractDate_WithCulture();
            ExtractDate_InvalidDateString();
            ExtractDate_WithNullEmptyString();
            ExtractDate_WithRegexPattern();
            ExtractInt_ValidInteger();
            ExtractInt_WithNumberStyles();
            ExtractInt_WithCulture();
            ExtractInt_InvalidIntegerString();
            ExtractInt_WithRegexPattern();
            ExtractDouble_ValidDouble();
            ExtractDouble_WithNumberStyles();
            ExtractDouble_WithCulture();
            ExtractDouble_InvalidDoubleString();
            ExtractDecimal_ValidDecimal();
            ExtractDecimal_WithNumberStyles();
            ExtractDecimal_WithCulture();
            ExtractDecimal_InvalidDecimalString();
            ExtractBool_TrueString();
            ExtractBool_FalseString();
            ExtractBool_InvalidBooleanString();
            ExtractGuid_ValidGuid();
            ExtractGuid_InvalidGuidString();
            ExtractGuid_WithRegexPattern();
        }

        public void Extract_WithNullSource_ReturnsInvalid()
        {
            // Arrange
            string source = null;
            var config = ExtractConfig.Default;

            // Act
            var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have errors");
        }


        public void Extract_WithEmptySource_ReturnsInvalid()
        {
            // Arrange
            string source = "";
            var config = ExtractConfig.Default;

            // Act
            var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void Extract_WithRegexPattern_Success()
        {
            // Arrange
            string source = "Price: $99.99";
            var config = new ExtractConfig
            {
                RegexPattern = @"Price: \$(\d+\.\d{2})",
                GroupIndex = 1
            };

            // Act
            var result = source.Extract<decimal>(config, (str, cfg) => decimal.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 99.99m, "Value should match");
        }


        public void Extract_WithRegexPattern_NoMatch()
        {
            // Arrange
            string source = "No price here";
            var config = new ExtractConfig
            {
                RegexPattern = @"Price: \$(\d+\.\d{2})",
                GroupIndex = 1
            };

            // Act
            var result = source.Extract<decimal>(config, (str, cfg) => decimal.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void Extract_WithRegexGroupIndex_Valid()
        {
            // Arrange
            string source = "ID: 12345 Name: Test";
            var config = new ExtractConfig
            {
                RegexPattern = @"ID: (\d+) Name: (\w+)",
                GroupIndex = 1
            };

            // Act
            var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 12345, "Value should match group 1");
        }


        public void Extract_WithRegexGroupIndex_OutOfRange()
        {
            // Arrange
            string source = "ID: 12345";
            var config = new ExtractConfig
            {
                RegexPattern = @"ID: (\d+)",
                GroupIndex = 5
            };

            // Act
            var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void Extract_WithRegexOptions_IgnoreCase()
        {
            // Arrange
            string source = "PRICE: $99.99";
            var config = new ExtractConfig
            {
                RegexPattern = @"price: \$(\d+\.\d{2})",
                GroupIndex = 1,
                RegexOptions = RegexOptions.IgnoreCase
            };

            // Act
            var result = source.Extract<decimal>(config, (str, cfg) => decimal.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid with IgnoreCase");
        }


        public void Extract_WithThrowOnFailure_True_ThrowsException()
        {
            // Arrange
            string source = "invalid";
            var config = new ExtractConfig
            {
                ThrowOnFailure = true
            };

            // Act & Assert
            try
            {
                var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));
                testing.VerifyExpression(false, "Should have thrown exception");
            }
            catch (InvalidOperationException)
            {
                testing.VerifyExpression(true, "Correctly threw exception");
            }
        }


        public void Extract_WithThrowOnFailure_False_ReturnsInvalid()
        {
            // Arrange
            string source = "invalid";
            var config = new ExtractConfig
            {
                ThrowOnFailure = false
            };

            // Act
            var result = source.Extract<int>(config, (str, cfg) => int.Parse(str));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void Extract_WithExceptionInConverter_HandledGracefully()
        {
            // Arrange
            string source = "test";
            var config = ExtractConfig.Default;

            // Act
            var result = source.Extract<int>(config, (str, cfg) => throw new Exception("Converter error"));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have error");
        }


        public void ExtractDate_ValidDateString_DefaultFormat()
        {
            // Arrange
            string source = "2024-12-10";

            // Act
            var result = source.ExtractDate();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Year == 2024, "Year should match");
            testing.VerifyExpression(result.Value.Month == 12, "Month should match");
            testing.VerifyExpression(result.Value.Day == 10, "Day should match");
        }


        public void ExtractDate_WithCustomDateTimeFormat()
        {
            // Arrange
            string source = "25/12/2024";
            var config = new ExtractConfig
            {
                DateTimeFormat = "dd/MM/yyyy"
            };

            // Act
            var result = source.ExtractDate(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Day == 25, "Day should match");
            testing.VerifyExpression(result.Value.Month == 12, "Month should match");
        }


        public void ExtractDate_WithDateTimeFormatsArray()
        {
            // Arrange
            string source = "12-25-2024";
            var config = new ExtractConfig
            {
                DateTimeFormats = new[] { "MM-dd-yyyy", "yyyy-MM-dd", "dd/MM/yyyy" }
            };

            // Act
            var result = source.ExtractDate(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDate_WithCulture()
        {
            // Arrange
            string source = "25.12.2024";
            var config = new ExtractConfig
            {
                DateTimeFormat = "dd.MM.yyyy",
                Culture = CultureInfo.GetCultureInfo("de-DE")
            };

            // Act
            var result = source.ExtractDate(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDate_InvalidDateString()
        {
            // Arrange
            string source = "invalid-date";

            // Act
            var result = source.ExtractDate();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractDate_WithNullEmptyString()
        {
            // Arrange
            string source = null;

            // Act
            var result = source.ExtractDate();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractDate_WithRegexPattern()
        {
            // Arrange
            string source = "Order date: 2024-12-10";
            var config = new ExtractConfig
            {
                RegexPattern = @"Order date: (\d{4}-\d{2}-\d{2})",
                GroupIndex = 1
            };

            // Act
            var result = source.ExtractDate(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Year == 2024, "Year should match");
        }


        public void ExtractInt_ValidInteger()
        {
            // Arrange
            string source = "42";

            // Act
            var result = source.ExtractInt();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 42, "Value should match");
        }


        public void ExtractInt_WithNumberStyles()
        {
            // Arrange
            string source = " 42 ";
            var config = new ExtractConfig
            {
                NumberStyles = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
            };

            // Act
            var result = source.ExtractInt(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 42, "Value should match");
        }


        public void ExtractInt_WithCulture()
        {
            // Arrange
            string source = "1,234";
            var config = new ExtractConfig
            {
                NumberStyles = NumberStyles.AllowThousands,
                Culture = CultureInfo.GetCultureInfo("en-US")
            };

            // Act
            var result = source.ExtractInt(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 1234, "Value should match");
        }


        public void ExtractInt_InvalidIntegerString()
        {
            // Arrange
            string source = "not-a-number";

            // Act
            var result = source.ExtractInt();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractInt_WithRegexPattern()
        {
            // Arrange
            string source = "Count: 42 items";
            var config = new ExtractConfig
            {
                RegexPattern = @"Count: (\d+)",
                GroupIndex = 1
            };

            // Act
            var result = source.ExtractInt(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 42, "Value should match");
        }


        public void ExtractDouble_ValidDouble()
        {
            // Arrange
            string source = "99.99";

            // Act
            var result = source.ExtractDouble();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(Math.Abs(result.Value - 99.99) < 0.001, "Value should match");
        }


        public void ExtractDouble_WithNumberStyles()
        {
            // Arrange
            string source = "+99.99";
            var config = new ExtractConfig
            {
                NumberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
            };

            // Act
            var result = source.ExtractDouble(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDouble_WithCulture()
        {
            // Arrange
            string source = "99,99";
            var config = new ExtractConfig
            {
                Culture = CultureInfo.GetCultureInfo("de-DE")
            };

            // Act
            var result = source.ExtractDouble(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDouble_InvalidDoubleString()
        {
            // Arrange
            string source = "not-a-double";

            // Act
            var result = source.ExtractDouble();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractDecimal_ValidDecimal()
        {
            // Arrange
            string source = "99.99";

            // Act
            var result = source.ExtractDecimal();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 99.99m, "Value should match");
        }


        public void ExtractDecimal_WithNumberStyles()
        {
            // Arrange
            string source = "$99.99";
            var config = new ExtractConfig
            {
                NumberStyles = NumberStyles.Currency,
                Culture = CultureInfo.GetCultureInfo("en-US")
            };

            // Act
            var result = source.ExtractDecimal(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDecimal_WithCulture()
        {
            // Arrange
            string source = "99,99";
            var config = new ExtractConfig
            {
                Culture = CultureInfo.GetCultureInfo("de-DE")
            };

            // Act
            var result = source.ExtractDecimal(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }


        public void ExtractDecimal_InvalidDecimalString()
        {
            // Arrange
            string source = "not-a-decimal";

            // Act
            var result = source.ExtractDecimal();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractBool_TrueString()
        {
            // Arrange
            string source = "true";

            // Act
            var result = source.ExtractBool();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == true, "Value should be true");
        }


        public void ExtractBool_FalseString()
        {
            // Arrange
            string source = "false";

            // Act
            var result = source.ExtractBool();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == false, "Value should be false");
        }


        public void ExtractBool_InvalidBooleanString()
        {
            // Arrange
            string source = "maybe";

            // Act
            var result = source.ExtractBool();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractGuid_ValidGuid()
        {
            // Arrange
            string source = "550e8400-e29b-41d4-a716-446655440000";

            // Act
            var result = source.ExtractGuid();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.ToString() == source, "Value should match");
        }


        public void ExtractGuid_InvalidGuidString()
        {
            // Arrange
            string source = "not-a-guid";

            // Act
            var result = source.ExtractGuid();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void ExtractGuid_WithRegexPattern()
        {
            // Arrange
            string source = "ID: 550e8400-e29b-41d4-a716-446655440000";
            var config = new ExtractConfig
            {
                RegexPattern = @"ID: ([a-f0-9-]+)",
                GroupIndex = 1
            };

            // Act
            var result = source.ExtractGuid(config);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
        }
    }
}

