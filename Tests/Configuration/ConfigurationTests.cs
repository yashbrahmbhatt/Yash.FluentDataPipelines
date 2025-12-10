using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;

namespace Yash.FluentDataPipelines
{
    public class ConfigurationTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            ExtractConfig_DefaultValues();
            ExtractConfig_CustomRegexPattern();
            ExtractConfig_CustomGroupIndex();
            ExtractConfig_CustomRegexOptions();
            ExtractConfig_CustomCulture();
            ExtractConfig_CustomDateTimeFormat();
            ExtractConfig_CustomDateTimeFormatsArray();
            ExtractConfig_CustomNumberStyles();
            ExtractConfig_ThrowOnFailureFlag();
            ValidationConfig_DefaultValues();
            ValidationConfig_CaseSensitiveFlag();
            ValidationConfig_StringComparisonOptions();
            ValidationConfig_ToleranceForNumericComparisons();
            ValidationConfig_InclusiveLowerBoundFlag();
            ValidationConfig_InclusiveUpperBoundFlag();
            ValidationConfig_CustomValidatorFunction();
            ValidationConfig_CustomErrorMessage();
            TransformationConfig_DefaultValues();
            TransformationConfig_RoundingModeOptions();
            TransformationConfig_CustomCulture();
            TransformationConfig_TrimWhitespaceFlag();
            TransformationConfig_TrimCharsArray();
            TransformationConfig_CustomTransformFunction();
            FormatConfig_DefaultValues();
            FormatConfig_FormatString();
            FormatConfig_CustomCulture();
            FormatConfig_CustomFormatterFunction();
            FormatConfig_CustomFormatterWithValidationFunction();
            FormatConfig_NullValueString();
            FormatConfig_InvalidValueString();
        }

        public void ExtractConfig_DefaultValues()
        {
            // Arrange & Act
            var config = ExtractConfig.Default;

            // Assert
            testing.VerifyExpression(config.RegexPattern == null, "RegexPattern should be null");
            testing.VerifyExpression(config.GroupIndex == 0, "GroupIndex should be 0");
            testing.VerifyExpression(config.RegexOptions == RegexOptions.None, "RegexOptions should be None");
            testing.VerifyExpression(config.Culture != null, "Culture should not be null");
            testing.VerifyExpression(config.DateTimeFormat == null, "DateTimeFormat should be null");
            testing.VerifyExpression(config.DateTimeFormats == null, "DateTimeFormats should be null");
            testing.VerifyExpression(config.DateTimeStyles == DateTimeStyles.None, "DateTimeStyles should be None");
            testing.VerifyExpression(config.NumberStyles == NumberStyles.Any, "NumberStyles should be Any");
            testing.VerifyExpression(config.ThrowOnFailure == false, "ThrowOnFailure should be false");
        }


        public void ExtractConfig_CustomRegexPattern()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                RegexPattern = @"\d+"
            };

            // Assert
            testing.VerifyExpression(config.RegexPattern == @"\d+", "RegexPattern should be set");
        }


        public void ExtractConfig_CustomGroupIndex()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                GroupIndex = 1
            };

            // Assert
            testing.VerifyExpression(config.GroupIndex == 1, "GroupIndex should be set");
        }


        public void ExtractConfig_CustomRegexOptions()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                RegexOptions = RegexOptions.IgnoreCase | RegexOptions.Multiline
            };

            // Assert
            testing.VerifyExpression(config.RegexOptions == (RegexOptions.IgnoreCase | RegexOptions.Multiline), "RegexOptions should be set");
        }


        public void ExtractConfig_CustomCulture()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                Culture = CultureInfo.GetCultureInfo("de-DE")
            };

            // Assert
            testing.VerifyExpression(config.Culture.Name == "de-DE", "Culture should be set");
        }


        public void ExtractConfig_CustomDateTimeFormat()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                DateTimeFormat = "dd/MM/yyyy"
            };

            // Assert
            testing.VerifyExpression(config.DateTimeFormat == "dd/MM/yyyy", "DateTimeFormat should be set");
        }


        public void ExtractConfig_CustomDateTimeFormatsArray()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                DateTimeFormats = new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" }
            };

            // Assert
            testing.VerifyExpression(config.DateTimeFormats.Length == 3, "DateTimeFormats should have 3 formats");
        }


        public void ExtractConfig_CustomNumberStyles()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                NumberStyles = NumberStyles.Currency
            };

            // Assert
            testing.VerifyExpression(config.NumberStyles == NumberStyles.Currency, "NumberStyles should be set");
        }


        public void ExtractConfig_ThrowOnFailureFlag()
        {
            // Arrange & Act
            var config = new ExtractConfig
            {
                ThrowOnFailure = true
            };

            // Assert
            testing.VerifyExpression(config.ThrowOnFailure == true, "ThrowOnFailure should be set");
        }


        public void ValidationConfig_DefaultValues()
        {
            // Arrange & Act
            var config = ValidationConfig.Default;

            // Assert
            testing.VerifyExpression(config.CaseSensitive == true, "CaseSensitive should be true");
            testing.VerifyExpression(config.StringComparison == StringComparison.Ordinal, "StringComparison should be Ordinal");
            testing.VerifyExpression(config.Tolerance == null, "Tolerance should be null");
            testing.VerifyExpression(config.InclusiveLowerBound == true, "InclusiveLowerBound should be true");
            testing.VerifyExpression(config.InclusiveUpperBound == true, "InclusiveUpperBound should be true");
            testing.VerifyExpression(config.CustomValidator == null, "CustomValidator should be null");
            testing.VerifyExpression(config.ErrorMessage == null, "ErrorMessage should be null");
        }


        public void ValidationConfig_CaseSensitiveFlag()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                CaseSensitive = false
            };

            // Assert
            testing.VerifyExpression(config.CaseSensitive == false, "CaseSensitive should be set");
        }


        public void ValidationConfig_StringComparisonOptions()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                StringComparison = StringComparison.OrdinalIgnoreCase
            };

            // Assert
            testing.VerifyExpression(config.StringComparison == StringComparison.OrdinalIgnoreCase, "StringComparison should be set");
        }


        public void ValidationConfig_ToleranceForNumericComparisons()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                Tolerance = 0.01
            };

            // Assert
            testing.VerifyExpression(config.Tolerance == 0.01, "Tolerance should be set");
        }


        public void ValidationConfig_InclusiveLowerBoundFlag()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                InclusiveLowerBound = false
            };

            // Assert
            testing.VerifyExpression(config.InclusiveLowerBound == false, "InclusiveLowerBound should be set");
        }


        public void ValidationConfig_InclusiveUpperBoundFlag()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                InclusiveUpperBound = false
            };

            // Assert
            testing.VerifyExpression(config.InclusiveUpperBound == false, "InclusiveUpperBound should be set");
        }


        public void ValidationConfig_CustomValidatorFunction()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                CustomValidator = value => ((int)value) % 2 == 0
            };

            // Assert
            testing.VerifyExpression(config.CustomValidator != null, "CustomValidator should be set");
            testing.VerifyExpression(config.CustomValidator(42) == true, "CustomValidator should work");
            testing.VerifyExpression(config.CustomValidator(41) == false, "CustomValidator should work");
        }


        public void ValidationConfig_CustomErrorMessage()
        {
            // Arrange & Act
            var config = new ValidationConfig
            {
                ErrorMessage = "Custom validation error"
            };

            // Assert
            testing.VerifyExpression(config.ErrorMessage == "Custom validation error", "ErrorMessage should be set");
        }


        public void TransformationConfig_DefaultValues()
        {
            // Arrange & Act
            var config = TransformationConfig.Default;

            // Assert
            testing.VerifyExpression(config.RoundingMode == MidpointRounding.ToEven, "RoundingMode should be ToEven");
            testing.VerifyExpression(config.Culture != null, "Culture should not be null");
            testing.VerifyExpression(config.TrimWhitespace == true, "TrimWhitespace should be true");
            testing.VerifyExpression(config.TrimChars == null, "TrimChars should be null");
            testing.VerifyExpression(config.CustomTransform == null, "CustomTransform should be null");
        }


        public void TransformationConfig_RoundingModeOptions()
        {
            // Arrange & Act
            var config = new TransformationConfig
            {
                RoundingMode = MidpointRounding.AwayFromZero
            };

            // Assert
            testing.VerifyExpression(config.RoundingMode == MidpointRounding.AwayFromZero, "RoundingMode should be set");
        }


        public void TransformationConfig_CustomCulture()
        {
            // Arrange & Act
            var config = new TransformationConfig
            {
                Culture = CultureInfo.GetCultureInfo("fr-FR")
            };

            // Assert
            testing.VerifyExpression(config.Culture.Name == "fr-FR", "Culture should be set");
        }


        public void TransformationConfig_TrimWhitespaceFlag()
        {
            // Arrange & Act
            var config = new TransformationConfig
            {
                TrimWhitespace = false
            };

            // Assert
            testing.VerifyExpression(config.TrimWhitespace == false, "TrimWhitespace should be set");
        }


        public void TransformationConfig_TrimCharsArray()
        {
            // Arrange & Act
            var config = new TransformationConfig
            {
                TrimChars = new[] { '*', '#' }
            };

            // Assert
            testing.VerifyExpression(config.TrimChars.Length == 2, "TrimChars should have 2 chars");
            testing.VerifyExpression(config.TrimChars[0] == '*', "First char should be *");
            testing.VerifyExpression(config.TrimChars[1] == '#', "Second char should be #");
        }


        public void TransformationConfig_CustomTransformFunction()
        {
            // Arrange & Act
            var config = new TransformationConfig
            {
                CustomTransform = value => ((int)value) * 2
            };

            // Assert
            testing.VerifyExpression(config.CustomTransform != null, "CustomTransform should be set");
            testing.VerifyExpression((int)config.CustomTransform(21) == 42, "CustomTransform should work");
        }


        public void FormatConfig_DefaultValues()
        {
            // Arrange & Act
            var config = FormatConfig.Default;

            // Assert
            testing.VerifyExpression(config.FormatString == null, "FormatString should be null");
            testing.VerifyExpression(config.Culture != null, "Culture should not be null");
            testing.VerifyExpression(config.CustomFormatter == null, "CustomFormatter should be null");
            testing.VerifyExpression(config.CustomFormatterWithValidation == null, "CustomFormatterWithValidation should be null");
            testing.VerifyExpression(config.NullValueString == string.Empty, "NullValueString should be empty");
            testing.VerifyExpression(config.InvalidValueString == "Invalid", "InvalidValueString should be 'Invalid'");
        }


        public void FormatConfig_FormatString()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                FormatString = "yyyy-MM-dd"
            };

            // Assert
            testing.VerifyExpression(config.FormatString == "yyyy-MM-dd", "FormatString should be set");
        }


        public void FormatConfig_CustomCulture()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                Culture = CultureInfo.GetCultureInfo("ja-JP")
            };

            // Assert
            testing.VerifyExpression(config.Culture.Name == "ja-JP", "Culture should be set");
        }


        public void FormatConfig_CustomFormatterFunction()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                CustomFormatter = value => $"Custom: {value}"
            };

            // Assert
            testing.VerifyExpression(config.CustomFormatter != null, "CustomFormatter should be set");
            testing.VerifyExpression(config.CustomFormatter(42) == "Custom: 42", "CustomFormatter should work");
        }


        public void FormatConfig_CustomFormatterWithValidationFunction()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                CustomFormatterWithValidation = (value, isValid) => isValid ? $"Valid: {value}" : $"Invalid: {value}"
            };

            // Assert
            testing.VerifyExpression(config.CustomFormatterWithValidation != null, "CustomFormatterWithValidation should be set");
            testing.VerifyExpression(config.CustomFormatterWithValidation(42, true) == "Valid: 42", "CustomFormatterWithValidation should work");
            testing.VerifyExpression(config.CustomFormatterWithValidation(42, false) == "Invalid: 42", "CustomFormatterWithValidation should work");
        }


        public void FormatConfig_NullValueString()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                NullValueString = "NULL"
            };

            // Assert
            testing.VerifyExpression(config.NullValueString == "NULL", "NullValueString should be set");
        }


        public void FormatConfig_InvalidValueString()
        {
            // Arrange & Act
            var config = new FormatConfig
            {
                InvalidValueString = "INVALID"
            };

            // Assert
            testing.VerifyExpression(config.InvalidValueString == "INVALID", "InvalidValueString should be set");
        }
    }
}

