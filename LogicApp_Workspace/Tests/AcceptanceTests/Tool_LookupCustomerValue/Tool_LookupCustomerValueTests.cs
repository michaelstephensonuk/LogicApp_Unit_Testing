using System.Text;
using System.Text.Json;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.Tool_LookupCustomerValue;



[TestClass]
public sealed class Tool_LookupCustomerValueTests
{

    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "Tool_LookupCustomerValue";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var args = new Dictionary<string, object>();
        args.Add("customer_id", "123");

        var input = JsonSerializer.Serialize(args);
        var content = new StringContent(input, Encoding.UTF8, "application/json");

        //Act
        var response = logicAppTestManager.TriggerLogicAppWithPost(content, triggerName: "When_a_HTTP_request_is_received");

        //Assert: Successful response
        Assert.IsNotNull(response);
        Assert.AreEqual(200, (int)response.HttpResponse.StatusCode);

        //Assert: Workflow run ID is not null
        Assert.IsNotNull(response.WorkFlowRunId);

        //Load Run History for more assertions
        logicAppTestManager.LoadWorkflowRunHistory();

        //Assert: Trigger was successful
        var triggerStatus = logicAppTestManager.GetTriggerStatus();
        Assert.AreEqual(triggerStatus, TriggerStatus.Succeeded);

        //Assert: Action setting sample response
        var actionStatus = logicAppTestManager.GetActionStatus("Compose");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Action calling function was successful
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);

        //Assert: Response content contains expected data
        var responseContent = response.HttpResponse.Content.ReadAsStringAsync().Result;
        Assert.IsNotNull(responseContent, "Response content should not be null");

        var responseArgs = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        Assert.IsNotNull(responseArgs, "Response content should be a valid JSON object");
        Assert.IsTrue(responseArgs.ContainsKey("CustomerValuePercentage"), "Response content should contain 'value' key");


        int actualCustomerValue = 0;
        responseArgs.TryGetValue("CustomerValuePercentage", out var value);
        Assert.IsTrue(int.TryParse(value.ToString(), out actualCustomerValue), "CustomerValuePercentage should be a valid integer");
       
        //Assert: The value is between 0 and 100
        Assert.IsTrue(actualCustomerValue >= 0 && actualCustomerValue <= 100, "The value should be between 0 and 100");
    }
}
