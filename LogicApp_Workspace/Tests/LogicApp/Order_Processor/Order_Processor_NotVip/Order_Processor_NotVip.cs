using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Workflows.Common.ErrorResponses;
using Microsoft.Azure.Workflows.UnitTesting;
using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using LogicApp.Tests.Mocks.Order_Processor;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class Order_Processor_NotVip
    {
        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("Order_Processor/testSettings.config");
        }

        /// <summary>
        /// Simple mocked test to validate that the path which is not a VIP path executes successfully.
        /// </summary>
        [TestMethod]
        public async Task Order_Processor_Not_Vip_GreenPath()
        {
            // PREPARE Mock
            // Generate mock action and trigger data.
            var mockData = this.GetTestMockDefinition();

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);

            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Actions["Agent_Process_Order"].Status,
                message: "The action 'Agent_Process_Order' should be successful.");

            //Get the condition action as its a parent of the other actions to test
            var conditionAction = testRun.Actions["Condition_-_Is_Vip"];

            //Assert: the blob storage action was executed
            var blobUploadAction = conditionAction.ChildActions["Upload_blob_to_storage_container_based_on_a_URI"];
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: blobUploadAction.Status,
                message: "The action uploading a blob to storage should be successful.");

            //Assert the cosmos db action was skipped
            var cosmosAction = conditionAction.ChildActions["Create_or_update_item"];
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: cosmosAction.Status,
                message: "The action uploading to CosmosDB should be skipped.");

        }


        /// <summary>
        /// A sample unit test for executing the workflow named Order_Processor with failed mocked data.
        /// This method shows how to set up mock data, execute the workflow, and assert the outcome.
        /// </summary>
        [TestMethod]
        public async Task Order_Processor_NotVip_Error_Executing_Agent_Decision_Maker()
        {
            // PREPARE
            // Generate mock action and trigger data.
            var mockData = this.GetTestMockDefinition();

            //We will override the default behaviour for the mock to simulate an error calling the child workflow
            var mockError = new TestErrorInfo(code: ErrorResponseCode.InternalServerError, message: "Error executing the agent");
            mockData.ActionMocks["Agent_Process_Order"] = new AgentProcessOrderActionMock(status: TestWorkflowStatus.Failed, error: mockError);

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Failed, actual: testRun.Status);


            //Assert: The agent loop call failed
            Assert.AreEqual(expected: TestWorkflowStatus.Failed, actual: testRun.Actions["Agent_Process_Order"].Status,
                message: "The action 'Agent_Process_Order' should be failed.");
        }

        #region Mock generator helpers

        /// <summary>
        /// Returns deserialized test mock data.  
        /// </summary>
        private TestMockDefinition GetTestMockDefinition()
        {
            var mockDataPath = Path.Combine(TestExecutor.rootDirectory, "Tests", TestExecutor.logicAppName, TestExecutor.workflow, "Order_Processor_NotVip", "Order_Processor_NotVip-mock.json");
            return JsonConvert.DeserializeObject<TestMockDefinition>(File.ReadAllText(mockDataPath));
        }

        /// <summary>
        /// The callback method to dynamically generate mocked data for the action named 'actionName'.
        /// You can modify this method to return different mock status, outputs, and error based on the test scenario.
        /// </summary>
        /// <param name="context">The test execution context that contains information about the current test run.</param>
        public AgentProcessOrderActionMock AgentProcessOrderActionMockOutputCallback(TestExecutionContext context)
        {
            // Sample mock data : Modify the existing mocked data dynamically for "actionName".
            return new AgentProcessOrderActionMock(
                status: TestWorkflowStatus.Succeeded,
                outputs: new AgentProcessOrderActionOutput {
                    // set the desired properties here
                    // if this acount contains a JObject Body
                    // Body = "something".ToJObject()
                }
            );
        }

        #endregion
    }
}