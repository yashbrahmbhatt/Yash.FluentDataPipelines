using System;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Core;

namespace Yash.FluentDataPipelines.Tests.Core
{
    public class NormalizationUtilitiesTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            NormalizeAddress_RemovesPunctuation();
            NormalizeAddress_ExpandsAbbreviations();
            NormalizeAddress_NormalizesWhitespace();
            NormalizeAddress_ToLowercase();
            NormalizeAddress_RemoveCommonWords();
            NormalizePhone_RemovesNonDigits();
            NormalizePhone_RemovesExtensions();
            NormalizePhone_PreservesDigits();
            NormalizeName_NormalizesWhitespace();
            NormalizeName_ToLowercase();
            NormalizeName_RemoveTitles();
            NormalizeString_GenericNormalization();
            NormalizeString_NullHandling();
        }

        public void NormalizeAddress_RemovesPunctuation()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeAddress("123 Main St., Apt. 4B");

            // Assert
            testing.VerifyExpression(!normalized.Contains("."), $"Should remove periods. Expected: result not to contain '.', Actual: '{normalized}'", true, "Should remove periods", false, false);
            testing.VerifyExpression(!normalized.Contains(","), $"Should remove commas. Expected: result not to contain ',', Actual: '{normalized}'", true, "Should remove commas", false, false);
        }

        public void NormalizeAddress_ExpandsAbbreviations()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeAddress("123 Main St");

            // Assert
            testing.VerifyExpression(normalized.Contains("street"), $"Should expand St to Street. Expected: result to contain 'street', Actual: '{normalized}'", true, "Should expand St to Street", false, false);
        }

        public void NormalizeAddress_NormalizesWhitespace()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeAddress("123   Main    Street");

            // Assert
            testing.VerifyExpression(!normalized.Contains("  "), $"Should normalize multiple spaces. Expected: result not to contain '  ', Actual: '{normalized}'", true, "Should normalize multiple spaces", false, false);
        }

        public void NormalizeAddress_ToLowercase()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeAddress("123 MAIN STREET");

            // Assert
            testing.VerifyExpression(normalized == normalized.ToLowerInvariant(), $"Should convert to lowercase. Expected: {normalized.ToLowerInvariant()}, Actual: {normalized}", true, "Should convert to lowercase", false, false);
        }

        public void NormalizeAddress_RemoveCommonWords()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeAddress("The 123 Main Street", toLowercase: true, removeCommonWords: true);

            // Assert
            testing.VerifyExpression(!normalized.Contains("the"), $"Should remove common words. Expected: result not to contain 'the', Actual: '{normalized}'", true, "Should remove common words", false, false);
        }

        public void NormalizePhone_RemovesNonDigits()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizePhone("(555) 123-4567");

            // Assert
            testing.VerifyExpression(normalized == "5551234567", $"Should contain only digits. Expected: '5551234567', Actual: '{normalized}'", true, "Should contain only digits", false, false);
        }

        public void NormalizePhone_RemovesExtensions()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizePhone("555-1234 x123");

            // Assert
            testing.VerifyExpression(!normalized.Contains("x"), $"Should remove extensions. Expected: result not to contain 'x', Actual: '{normalized}'", true, "Should remove extensions", false, false);
            testing.VerifyExpression(normalized == "5551234", $"Should only contain main number. Expected: '5551234', Actual: '{normalized}'", true, "Should only contain main number", false, false);
        }

        public void NormalizePhone_PreservesDigits()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizePhone("5551234567");

            // Assert
            testing.VerifyExpression(normalized == "5551234567", $"Should preserve digits. Expected: '5551234567', Actual: '{normalized}'", true, "Should preserve digits", false, false);
        }

        public void NormalizeName_NormalizesWhitespace()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeName("John   Smith");

            // Assert
            testing.VerifyExpression(!normalized.Contains("  "), $"Should normalize whitespace. Expected: result not to contain '  ', Actual: '{normalized}'", true, "Should normalize whitespace", false, false);
        }

        public void NormalizeName_ToLowercase()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeName("JOHN SMITH");

            // Assert
            testing.VerifyExpression(normalized == normalized.ToLowerInvariant(), $"Should convert to lowercase. Expected: {normalized.ToLowerInvariant()}, Actual: {normalized}", true, "Should convert to lowercase", false, false);
        }

        public void NormalizeName_RemoveTitles()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeName("Dr. John Smith", removeTitles: true);

            // Assert
            testing.VerifyExpression(!normalized.Contains("dr"), $"Should remove titles. Expected: result not to contain 'dr', Actual: '{normalized}'", true, "Should remove titles", false, false);
        }

        public void NormalizeString_GenericNormalization()
        {
            // Arrange & Act
            string normalized = NormalizationUtilities.NormalizeString("  TEST   STRING  ", toLowercase: true, removePunctuation: false, normalizeWhitespace: true);

            // Assert
            testing.VerifyExpression(normalized == "test string", $"Should normalize correctly. Expected: 'test string', Actual: '{normalized}'", true, "Should normalize correctly", false, false);
        }

        public void NormalizeString_NullHandling()
        {
            // Arrange & Act
            string normalized1 = NormalizationUtilities.NormalizeString(null);
            string normalized2 = NormalizationUtilities.NormalizeString("");

            // Assert
            testing.VerifyExpression(normalized1 == "", $"Null should return empty string. Expected: '', Actual: '{normalized1}'", true, "Null should return empty string", false, false);
            testing.VerifyExpression(normalized2 == "", $"Empty should return empty string. Expected: '', Actual: '{normalized2}'", true, "Empty should return empty string", false, false);
        }
    }
}

