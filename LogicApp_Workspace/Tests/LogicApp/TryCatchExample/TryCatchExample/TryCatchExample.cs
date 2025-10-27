using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LogicApp.Tests.Mocks.TryCatchExample;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Microsoft.Azure.Workflows.Common.ErrorResponses;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class TryCatchExample
    {
        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("TryCatchExample/testSettings.config");
        }

        /// <summary>
        /// An example where we successfully call google and we should skip the catch block and just return a good response
        /// </summary>
        [TestMethod]
        public async Task GreenPath()
        {
            var actionMocks = new Dictionary<string, ActionMock>();

            // PREPARE Mock
            // Generate mock trigger data.
            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);

            // Mock HTTP call to Google
            var actionMockOutput = new HTTPActionOutput();
            actionMockOutput.StatusCode = System.Net.HttpStatusCode.OK;
            var actionMock = new HTTPActionMock(name: "HTTP_-_Get_from_Google", outputs: actionMockOutput);

            actionMocks.Add(actionMock.Name, actionMock);

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: actionMocks);

            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);

            //Get references for the try and catch scopes so we can get their child actions
            var tryAction = testRun.Actions["Scope_-_Try"];
            var catchAction = testRun.Actions["Scope_-_Catch"];

            // Assert that the try block executed successfully.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: tryAction.Status,
                message: "The try block should be successful.");

            // Assert that the HTTP action within the try block executed successfully.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: tryAction.ChildActions["HTTP_-_Get_from_Google"].Status,
                message: "The action 'HTTP_-_Get_from_Google' should be successful.");

            // Assert that the HTTP action within the try block executed successfully.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: tryAction.ChildActions["Response"].Status,
                message: "The action 'Response' should be successful.");

            // Assert that the catch block was skipped.
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: catchAction.Status,
                message: "The catch block should be skipped.");
        }

        /// <summary>
        /// An example where we successfully call google and we should skip the catch block and just return a good response
        /// </summary>
        [TestMethod]
        public async Task Error_Calling_Google()
        {
            var actionMocks = new Dictionary<string, ActionMock>();

            // PREPARE Mock
            // Generate mock trigger data.
            var triggerMockOutput = new WhenAHTTPRequestIsReceivedTriggerOutput();
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(outputs: triggerMockOutput);

            // Mock HTTP call to Google
            //var actionMockOutput = new HTTPActionOutput();
            //actionMockOutput.StatusCode = System.Net.HttpStatusCode.BadRequest;  //This should cause an error

            var actionMock = new HTTPActionMock(name: "HTTP_-_Get_from_Google",
                                                status: TestWorkflowStatus.Failed,
                                                error: new TestErrorInfo(
                                                    message: "Error calling Google",
                                                    code: ErrorResponseCode.BadRequest
                                                ));
            actionMocks.Add(actionMock.Name, actionMock);

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testMock = new TestMockDefinition(
                triggerMock: triggerMock,
                actionMocks: actionMocks);

            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: testMock).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);

            //Get references for the try and catch scopes so we can get their child actions
            var tryAction = testRun.Actions["Scope_-_Try"];
            var catchAction = testRun.Actions["Scope_-_Catch"];

            // Assert that the HTTP action within the try block executed with an error.
            Assert.AreEqual(expected: TestWorkflowStatus.Failed, actual: tryAction.ChildActions["HTTP_-_Get_from_Google"].Status,
                message: "The action 'HTTP_-_Get_from_Google' should be failed.");

            // Assert that the try block got an error
            Assert.AreEqual(expected: TestWorkflowStatus.Failed, actual: tryAction.Status,
                message: "The try block should be failed.");
            

            // Assert that the HTTP action within the try block executed successfully.
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: tryAction.ChildActions["Response"].Status,
                message: "The action 'Response' should be skipped.");

            // Assert that the catch block was skipped.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: catchAction.Status,
                message: "The catch block should be succeeded.");

            // Assert the response action returning an error should be executed
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: catchAction.ChildActions["Response_-_Error"].Status,
                message: "The action 'Response' should be Succeeded.");
        }

        
    }
}