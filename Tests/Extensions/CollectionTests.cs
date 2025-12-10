using System;
using System.Collections.Generic;
using System.Linq;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using Yash.FluentDataPipelines.Configuration;
using Yash.FluentDataPipelines.Core;
using Yash.FluentDataPipelines.Extensions;

namespace Yash.FluentDataPipelines
{
    public class CollectionTests : CodedWorkflow
    {
        [TestCase]
        public void Execute()
        {
            Transform_CollectionWithValidPipelineValue();
            Transform_CollectionWithInvalidPipelineValue();
            Transform_CollectionWithNullPipelineValue();
            Transform_CollectionWithException();
            Where_FilterCollectionWithPredicate();
            Where_FilterEmptyCollection();
            Where_FilterWithNoMatches();
            Where_FilterWithAllMatches();
            Select_ProjectCollectionElements();
            Select_ProjectEmptyCollection();
            Select_ProjectWithTypeConversion();
            First_ElementFromNonEmptyCollection();
            First_FromEmptyCollection_Throws();
            First_WithInvalidPipelineValue();
            FirstOrDefault_FromNonEmptyCollection();
            FirstOrDefault_FromEmptyCollection_ReturnsDefault();
            FirstOrDefault_WithCustomDefaultValue();
            Last_ElementFromNonEmptyCollection();
            Last_FromEmptyCollection_Throws();
            Last_WithInvalidPipelineValue();
            LastOrDefault_FromNonEmptyCollection();
            LastOrDefault_FromEmptyCollection_ReturnsDefault();
            LastOrDefault_WithCustomDefaultValue();
            Where_ThenSelect_ThenFirst_Chaining();
        }

        public void Transform_CollectionWithValidPipelineValue()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);
            var config = TransformationConfig.Default;

            // Act
            var result = pipelineValue.Transform(config, (collection, cfg) => collection.Sum());

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 15, "Sum should be 15");
        }


        public void Transform_CollectionWithInvalidPipelineValue()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, false, new[] { new PipelineError("Error", "Op") });
            var config = TransformationConfig.Default;

            // Act
            var result = pipelineValue.Transform(config, (collection, cfg) => collection.Sum());

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should remain invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should preserve errors");
        }


        public void Transform_CollectionWithNullPipelineValue()
        {
            // Arrange
            PipelineValue<IEnumerable<int>> pipelineValue = null;
            var config = TransformationConfig.Default;

            // Act
            var result = pipelineValue.Transform(config, (collection, cfg) => collection.Sum());

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
        }


        public void Transform_CollectionWithException()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);
            var config = TransformationConfig.Default;

            // Act
            var result = pipelineValue.Transform(config, (collection, cfg) => throw new Exception("Transform error"));

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have error");
        }


        public void Where_FilterCollectionWithPredicate()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Where(x => x > 2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Count() == 3, "Should have 3 elements");
            testing.VerifyExpression(result.Value.All(x => x > 2), "All elements should be > 2");
        }


        public void Where_FilterEmptyCollection()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Where(x => x > 2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Count() == 0, "Should be empty");
        }


        public void Where_FilterWithNoMatches()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Where(x => x > 10);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Count() == 0, "Should have no matches");
        }


        public void Where_FilterWithAllMatches()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Where(x => x > 0);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Count() == 3, "Should have all matches");
        }


        public void Select_ProjectCollectionElements()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Select(x => x * 2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.SequenceEqual(new[] { 2, 4, 6 }), "Should project correctly");
        }


        public void Select_ProjectEmptyCollection()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Select(x => x * 2);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.Count() == 0, "Should be empty");
        }


        public void Select_ProjectWithTypeConversion()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Select(x => x.ToString());

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value.SequenceEqual(new[] { "1", "2", "3" }), "Should convert types");
        }


        public void First_ElementFromNonEmptyCollection()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.First();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 1, "Should return first element");
        }


        public void First_FromEmptyCollection_Throws()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.First();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have error");
        }


        public void First_WithInvalidPipelineValue()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, false, new[] { new PipelineError("Error", "Op") });

            // Act
            var result = pipelineValue.First();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should remain invalid");
        }


        public void FirstOrDefault_FromNonEmptyCollection()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.FirstOrDefault();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 1, "Should return first element");
        }


        public void FirstOrDefault_FromEmptyCollection_ReturnsDefault()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.FirstOrDefault();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 0, "Should return default(int)");
        }


        public void FirstOrDefault_WithCustomDefaultValue()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.FirstOrDefault(-1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == -1, "Should return custom default");
        }


        public void Last_ElementFromNonEmptyCollection()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Last();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 5, "Should return last element");
        }


        public void Last_FromEmptyCollection_Throws()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.Last();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should be invalid");
            testing.VerifyExpression(result.Errors.Count > 0, "Should have error");
        }


        public void Last_WithInvalidPipelineValue()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, false, new[] { new PipelineError("Error", "Op") });

            // Act
            var result = pipelineValue.Last();

            // Assert
            testing.VerifyExpression(result.IsValid == false, "Should remain invalid");
        }


        public void LastOrDefault_FromNonEmptyCollection()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.LastOrDefault();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 5, "Should return last element");
        }


        public void LastOrDefault_FromEmptyCollection_ReturnsDefault()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.LastOrDefault();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 0, "Should return default(int)");
        }


        public void LastOrDefault_WithCustomDefaultValue()
        {
            // Arrange
            var numbers = new int[0];
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue.LastOrDefault(-1);

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == -1, "Should return custom default");
        }


        public void Where_ThenSelect_ThenFirst_Chaining()
        {
            // Arrange
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var pipelineValue = new PipelineValue<IEnumerable<int>>(numbers, true);

            // Act
            var result = pipelineValue
                .Where(x => x > 2)
                .Select(x => x * 2)
                .First();

            // Assert
            testing.VerifyExpression(result.IsValid == true, "Should be valid");
            testing.VerifyExpression(result.Value == 6, "Should be first element after filter and transform");
        }
    }
}

