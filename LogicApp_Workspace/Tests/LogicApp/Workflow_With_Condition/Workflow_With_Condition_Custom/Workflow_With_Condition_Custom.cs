using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicApp.Tests.Mocks.Workflow_With_Condition;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class Workflow_With_Condition_Custom
    {
        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("Workflow_With_Condition/testSettings.config");
        }

        [TestMethod]
        public async Task YouWentLeft()
        {
            // Arrange
            Dictionary<string, ActionMock> actionMocks = new Dictionary<string, ActionMock>();

            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            triggerMockOutput.Body = new WhenAHTTPRequestIsReceivedTriggerOutputBody();
            triggerMockOutput.Body.Direction = "left";
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);

    
            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: null);
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status); //Workflow succeeded

            var conditionAction = testRun.Actions["Condition"];
            
            //Assert: Response Set Variable Action LEFT Was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: conditionAction.ChildActions["Compose_-_Left"].Status);

            //Assert: Response Set Variable RIGHT Action Was skipped
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: conditionAction.ChildActions["Compose_-_Right"].Status);

            //Assert: Response Action Was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Actions["Response"].Status);

            // Assert: Response body is as expected
            var actualResponseBody = testRun.Actions["Response"].Outputs["body"].ToString();

            Assert.IsTrue(actualResponseBody.Contains("You went left"), "The response body does not contain the expected text");
        }
        
        [TestMethod]
        public async Task YouWentRight()
        {
            // Arrange
            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            triggerMockOutput.Body = new WhenAHTTPRequestIsReceivedTriggerOutputBody();
            triggerMockOutput.Body.Direction = "right";

            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);


            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: null);
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status); //Workflow succeeded

            var conditionAction = testRun.Actions["Condition"];

            //Assert: Response Set Variable Action LEFT Was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: conditionAction.ChildActions["Set_variable_-_right"].Status);

            //Assert: Response Set Variable RIGHT Action Was skipped
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: conditionAction.ChildActions["Set_variable_-_left"].Status);

            //Assert: Response Action Was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Actions["Response"].Status);

            // Assert: Response body is as expected
            var actualResponseBody = testRun.Actions["Response"].Outputs["body"].ToString();

            Assert.IsTrue(actualResponseBody.Contains("You went right"), "The response body does not contain the expected text");
        }
    }
}