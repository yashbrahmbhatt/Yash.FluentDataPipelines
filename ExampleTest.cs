using System;
using System.Collections.Generic;
using System.Data;
using UiPath.Activities.System.Jobs.Coded;
using UiPath.CodedWorkflows;
using UiPath.Core;
using UiPath.Core.Activities.Storage;
using UiPath.Orchestrator.Client.Models;

namespace Yash.FluentDataPipelines
{
    // Test workflows in UiPath must be implemented as a class that derives from theb ase CodedWorkflow.
    public class ExampleTest : CodedWorkflow
    {
        // Test workflows are entered through the function with the [TestCase] attribute in the workflow class.
        // Other methods and members are allowed, but note that the constructor MUST be parameterless.
        // This usually means inputs and outputs are handled by the arguments of the [TestCase] function.
        [TestCase]
        public void Execute()
        {
            // Arrange
            Log("Test run started for ExampleTest.");

            // Act
            // For accessing UI Elements from Object Repository, you can use the Descriptors class e.g:
            // var screen = uiAutomation.Open(Descriptors.MyApp.FirstScreen);
            // screen.Click(Descriptors.MyApp.FirstScreen.SettingsButton);

            // Assert
            // To start using activities, use IntelliSense (CTRL + Space) to discover the available services, e.g. testing.VerifyExpression(...).
        }
    }
}