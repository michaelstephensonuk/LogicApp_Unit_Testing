using System.Text;
using System.Text.Json;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.Tool_LookUpCustomerHistory;



[TestClass]
public sealed class Tool_LookUpCustomerHistoryTests
{

    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "Tool_LookUpCustomerHistory";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var args = new Dictionary<string, object>();
        args.Add("CustomerID", "123");

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

        //Assert: Action calling to get sample test data was successful
        var actionStatus = logicAppTestManager.GetActionStatus("Get_Sample_Test_Data");
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
    }
}
