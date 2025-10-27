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
using LogicApp.Tests.Mocks.Order_DecisionMaker;
using Newtonsoft.Json.Linq;
using NLog.Fluent;
using LogicApp.Helpers;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class Order_DecisionMaker
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("Order_DecisionMaker/testSettings.config");
        }



        /// <summary>
        /// This should let us test the logic app to workout the customer is a VIP
        /// </summary>
        [TestMethod]
        public async Task GreenPath_HighValue_Should_Be_VIP()
        {
            var customerValue = 99; //  👈 - Means the customer is high value;
            var customerHistoryExecuted = false;
            var customerValueExecuted = false;

            // PREPARE
            // Generate mock action and trigger data from the test file
            var mockData = this.GetTestMockDefinition();

            //Setup Get Customer History Mock
            mockData.ActionMocks["Get_customer_history"] = new GetCustomerHistoryActionMock(
                name: "Get_customer_history",
                onGetActionMock: (testExecutionContext) =>
            {
                customerHistoryExecuted = true;

                return new GetCustomerHistoryActionMock(
                    status: TestWorkflowStatus.Succeeded,
                    outputs: new GetCustomerHistoryActionOutput
                    {
                        Body = File.ReadAllText(@"..\..\..\Order_DecisionMaker/Order_DecisionMaker/CustomerHistory.Sample.1.txt")
                    }
                );
            });

            //Setup Get Customer History Mock
            mockData.ActionMocks["Get_customer_value"] = new GetCustomerValueActionMock(
                name: "Get_customer_value",
                onGetActionMock: (testExecutionContext) =>
            {
                customerValueExecuted = true;

                return new GetCustomerValueActionMock(
                    status: TestWorkflowStatus.Succeeded,
                    outputs: new GetCustomerValueActionOutput
                    {
                        Body = new JObject
                        {
                            ["CustomerValuePercentage"] = customerValue
                        }
                    }
                );
            });


            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);

            LogicAppTestingHelper.LogTestRunInfo(this.TestContext, testRun);

            //ASSERT: The workflow run status is 'Succeeded'.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);

            //ASSERT: The AI Agent Loop action was successful
            var agentAction = testRun.Actions["Default_Agent"];
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.Status,
                message: "The action 'Default_Agent' should be successful.");

            //ASSERT: The Response action was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Actions["Response"].Status,
                message: "The action 'Response' should be successful.");

            //ISSUE: We cant access tool execution so we will need to workaround this
            //ASSERT: The Get_customer_history action was successful
            //Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.ChildActions["Get_customer_history"].Status,
            //    message: "The action 'Get_customer_history' should be successful.");

            //ASSERT: The Get_customer_value action was successful
            //Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.ChildActions["Get_customer_value"].Status,
            //    message: "The action 'Get_customer_value' should be successful.");

            //Assert the tools were executed
            Assert.IsTrue(customerHistoryExecuted, "The Get_customer_history action should have been executed.");
            Assert.IsTrue(customerValueExecuted, "The Get_customer_value action should have been executed.");

            //Assert response returned to caller
            var responseOutput = testRun.Actions["Response"].Outputs;
            var responseBody = responseOutput.SelectToken("$.body");
            var responseBodyJson = JObject.Parse(responseBody.ToString());
            var isVipValue = responseBodyJson.SelectToken("$.is_vip");
            var isVip = bool.Parse(isVipValue?.ToString() ?? "false");

            Assert.IsTrue(isVip, "The response SHOULD indicate the customer is a VIP.");
        }

        /// <summary>
        /// The aim here is to similate an error in one of the tools executed by the AI Agent
        /// This does not seem to work because the AI Agent is not able to simulate the error in this mode of testing.
        /// </summary>
        [TestMethod]
        public async Task Error_A_Tool_Failed()
        {
            var customerHistoryExecuted = false;
            var customerValueExecuted = false;

            // PREPARE
            // Generate mock action and trigger data.
            var mockData = this.GetTestMockDefinition();

            //Setup Get Customer History Mock to return an error
            mockData.ActionMocks["Get_customer_history"] = new GetCustomerHistoryActionMock(
                name: "Get_customer_history",
                onGetActionMock: (testExecutionContext) =>
            {
                customerHistoryExecuted = true;

                return new GetCustomerHistoryActionMock(
                    status: TestWorkflowStatus.Failed,
                    error: new TestErrorInfo(code: ErrorResponseCode.BadRequest, message: "Input is invalid.")
                );
            });

            //Setup Get Customer History Mock to return success
            mockData.ActionMocks["Get_customer_value"] = new GetCustomerValueActionMock(
                name: "Get_customer_value",
                onGetActionMock: (testExecutionContext) =>
            {
                customerValueExecuted = true;

                return new GetCustomerValueActionMock(
                    status: TestWorkflowStatus.Failed,
                    error: new TestErrorInfo(code: ErrorResponseCode.BadRequest, message: "Input is invalid.")
                );
            });

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);


            //ISSUE: When one of the tools fails it doesnt necessarily fail the Agent so this test
            //can not be reliable


            //Assert the tools were executed
            Assert.IsTrue(customerHistoryExecuted, "The Get_customer_history action should have been executed.");
            Assert.IsTrue(customerValueExecuted, "The Get_customer_value action should have been executed.");
            
            //ASSERT: The workflow run status is 'Failed'.
            Assert.AreEqual(expected: TestWorkflowStatus.Failed, actual: testRun.Status);
        }

        /// <summary>
        /// This should let us test the logic app to workout the customer is NOT a VIP
        /// </summary>
        [TestMethod]
        public async Task GreenPath_LowValue_Should_NOT_Be_VIP()
        {
            var customerValue = 5; //  👈 - Means the customer is low value;
            var customerHistoryExecuted = false;
            var customerValueExecuted = false;

            // PREPARE
            // Generate mock action and trigger data from the test file
            var mockData = this.GetTestMockDefinition();

            //Setup Get Customer History Mock
            mockData.ActionMocks["Get_customer_history"] = new GetCustomerHistoryActionMock(
                name: "Get_customer_history",
                onGetActionMock: (testExecutionContext) =>
            {
                customerHistoryExecuted = true;

                return new GetCustomerHistoryActionMock(
                    status: TestWorkflowStatus.Succeeded,
                    outputs: new GetCustomerHistoryActionOutput
                    {
                        Body = File.ReadAllText(@"..\..\..\Order_DecisionMaker/Order_DecisionMaker/CustomerHistory.Sample.1.txt")
                    }
                );
            });

            //Setup Get Customer History Mock
            mockData.ActionMocks["Get_customer_value"] = new GetCustomerValueActionMock(
                name: "Get_customer_value",
                onGetActionMock: (testExecutionContext) =>
            {
                customerValueExecuted = true;

                return new GetCustomerValueActionMock(
                    status: TestWorkflowStatus.Succeeded,
                    outputs: new GetCustomerValueActionOutput
                    {
                        Body = new JObject
                        {
                            ["CustomerValuePercentage"] = customerValue
                        }
                    }
                );
            });


            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);

            LogicAppTestingHelper.LogTestRunInfo(this.TestContext, testRun);

            //ASSERT: The workflow run status is 'Succeeded'.
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);

            //ASSERT: The AI Agent Loop action was successful
            var agentAction = testRun.Actions["Default_Agent"];
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.Status,
                message: "The action 'Default_Agent' should be successful.");

            //ASSERT: The Response action was successful
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Actions["Response"].Status,
                message: "The action 'Response' should be successful.");

            //ISSUE: We cant access tool execution so we will need to workaround this
            //ASSERT: The Get_customer_history action was successful
            //Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.ChildActions["Get_customer_history"].Status,
            //    message: "The action 'Get_customer_history' should be successful.");

            //ASSERT: The Get_customer_value action was successful
            //Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: agentAction.ChildActions["Get_customer_value"].Status,
            //    message: "The action 'Get_customer_value' should be successful.");

            //Assert the tools were executed
            Assert.IsTrue(customerHistoryExecuted, "The Get_customer_history action should have been executed.");
            Assert.IsTrue(customerValueExecuted, "The Get_customer_value action should have been executed.");

            //Assert response returned to caller
            var responseOutput = testRun.Actions["Response"].Outputs;
            var responseBody = responseOutput.SelectToken("$.body");
            var responseBodyJson = JObject.Parse(responseBody.ToString());
            var isVipValue = responseBodyJson.SelectToken("$.is_vip");
            var isVip = bool.Parse(isVipValue?.ToString() ?? "false");

            Assert.IsFalse(isVip, "The response should indicate the customer is a VIP when they should NOT be.");
        }

        #region Mock generator helpers

        /// <summary>
        /// Returns deserialized test mock data.  
        /// </summary>
        private TestMockDefinition GetTestMockDefinition()
        {
            var mockDataPath = Path.Combine(TestExecutor.rootDirectory, "Tests", TestExecutor.logicAppName, TestExecutor.workflow, "Order_DecisionMaker", "Order_DecisionMaker-mock.json");
            return JsonConvert.DeserializeObject<TestMockDefinition>(File.ReadAllText(mockDataPath));
        }

        /// <summary>
        /// The callback method to dynamically generate mocked data for the action named 'actionName'.
        /// You can modify this method to return different mock status, outputs, and error based on the test scenario.
        /// </summary>
        /// <param name="context">The test execution context that contains information about the current test run.</param>
        public GetCustomerHistoryActionMock GetCustomerHistoryActionMockOutputCallback(TestExecutionContext context)
        {
            // Sample mock data : Modify the existing mocked data dynamically for "actionName".
            return new GetCustomerHistoryActionMock(
                status: TestWorkflowStatus.Succeeded,
                outputs: new GetCustomerHistoryActionOutput {
                    // set the desired properties here
                    // if this acount contains a JObject Body
                    // Body = "something".ToJObject()
                }
            );
        }

        #endregion
    }
}