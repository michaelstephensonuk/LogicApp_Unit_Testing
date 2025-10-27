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
using LogicApp.Tests.Mocks.Tool_LookUpCustomerValue;

namespace LogicApp.Tests
{
    /// <summary>
    /// The unit test class.
    /// </summary>
    [TestClass]
    public class Tool_LookUpCustomerValue
    {
        /// <summary>
        /// The unit test executor.
        /// </summary>
        public TestExecutor TestExecutor;

        [TestInitialize]
        public void Setup()
        {
            this.TestExecutor = new TestExecutor("Tool_LookUpCustomerValue/testSettings.config");
        }

        /// <summary>
        /// A sample unit test for executing the workflow named Tool_LookUpCustomerValue with static mocked data.
        /// This method shows how to set up mock data, execute the workflow, and assert the outcome.
        /// </summary>
        [TestMethod]
        public async Task Tool_LookUpCustomerValue_Tool_LookUpCustomerValue_ExecuteWorkflow_SUCCESS_Sample1()
        {
            // PREPARE Mock
            // Generate mock action and trigger data.
            var mockData = this.GetTestMockDefinition();
            // mockData.TriggerMock.Outputs["your-property-name"] = "your-property-value";

            // ACT
            // Create an instance of UnitTestExecutor, and run the workflow with the mock data.
            var testRun = await this.TestExecutor
                .Create()
                .RunWorkflowAsync(testMock: mockData).ConfigureAwait(continueOnCapturedContext: false);

            // ASSERT
            // Verify that the workflow executed successfully, and the status is 'Succeeded'.
            Assert.IsNotNull(value: testRun);
            Assert.AreEqual(expected: TestWorkflowStatus.Succeeded, actual: testRun.Status);
        }

        #region Mock generator helpers

        /// <summary>
        /// Returns deserialized test mock data.  
        /// </summary>
        private TestMockDefinition GetTestMockDefinition()
        {
            var mockDataPath = Path.Combine(TestExecutor.rootDirectory, "Tests", TestExecutor.logicAppName, TestExecutor.workflow, "Tool_LookUpCustomerValue", "Tool_LookUpCustomerValue-mock.json");
            return JsonConvert.DeserializeObject<TestMockDefinition>(File.ReadAllText(mockDataPath));
        }

        #endregion
    }
}