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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Management.Automation;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class Order_Processor_IsVip
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
        public async Task Order_Processor_Is_Vip_GreenPath()
        {
            var actionMocks = new Dictionary<string, ActionMock>();

            // PREPARE Mock for Trigger
            var triggerMock = new WhenAHTTPRequestIsReceivedTriggerMock(
                status: TestWorkflowStatus.Succeeded,
                name: "When_a_HTTP_request_is_received",
                new WhenAHTTPRequestIsReceivedTriggerOutput());

            //Prepare mock for Agent Process Order Action
            var agentMockOutputBody = new Dictionary<string, object>();
            agentMockOutputBody.Add("is_vip", true);
            agentMockOutputBody.Add("description", "");

            actionMocks["Agent_Process_Order"] = new AgentProcessOrderActionMock(
                status: TestWorkflowStatus.Succeeded,
                outputs: new AgentProcessOrderActionOutput()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Body = JObject.FromObject(agentMockOutputBody)
                }
            );

            //Prepare mock for cosmos db Action
            actionMocks["Create_or_update_item"] = new CreateOrUpdateItemActionMock(
                status: TestWorkflowStatus.Succeeded,
                outputs: new CreateOrUpdateItemActionOutput()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Body = new CreateOrUpdateItemActionOutputBody()
                });

            //actionMocks["Create_or_update_item"] = null;

            //Setup Mock Definition
            var mockData = new TestMockDefinition(triggerMock: triggerMock, actionMocks: actionMocks);

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

            //Assert: the blob storage action was skipped
            var blobUploadAction = conditionAction.ChildActions["Upload_blob_to_storage_container_based_on_a_URI"];
            Assert.AreEqual(expected: TestWorkflowStatus.Skipped, actual: blobUploadAction.Status,
                message: "The action uploading a blob to storage should be successful.");

            //Assert the cosmos db action was executed
            var cosmosAction = conditionAction.ChildActions["Create_or_update_item"];
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: cosmosAction.Status,
                message: "The action uploading to CosmosDB should be skipped.");

        }

        
    }
}