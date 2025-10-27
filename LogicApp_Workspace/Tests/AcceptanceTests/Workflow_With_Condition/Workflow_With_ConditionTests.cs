using System.Text;
using System.Text.Json;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.Workflow_With_Condition;



[TestClass]
public sealed class Workflow_With_ConditionTests
{

    [TestMethod]
    public void Condition_Went_Left()
    {
        //Arrange
        var workflowName = "Workflow_With_Condition";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var args = new Dictionary<string, object>();
        args.Add("direction", "left");

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

        //Assert: Action setting the variable to left succeeded
        var actionStatus = logicAppTestManager.GetActionStatus("Set_variable_-_left");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Action setting the variable to right is skipped
        actionStatus = logicAppTestManager.GetActionStatus("Set_variable_-_right");
        Assert.AreEqual(actionStatus, ActionStatus.Skipped);

        //Assert: Action calling function was successful
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);

        //Assert: Response content contains expected data
        var responseContent = response.HttpResponse.Content.ReadAsStringAsync().Result;
        Assert.IsTrue(responseContent.Contains("You went left"));
    }
    

    [TestMethod]
    public void Condition_Went_Right()
    {
        //Arrange
        var workflowName = "Workflow_With_Condition";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var args = new Dictionary<string, object>();
        args.Add("direction", "right");

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

        //Assert: Action setting the variable to right succeeded
        var actionStatus = logicAppTestManager.GetActionStatus("Set_variable_-_right");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Action setting the variable to left is skipped
        actionStatus = logicAppTestManager.GetActionStatus("Set_variable_-_left");
        Assert.AreEqual(actionStatus, ActionStatus.Skipped);

        //Assert: Action calling function was successful
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
        
        //Assert: Response content contains expected data
        var responseContent = response.HttpResponse.Content.ReadAsStringAsync().Result;
        Assert.IsTrue(responseContent.Contains("You went right"));
    }
}
