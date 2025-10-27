using System.Text;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.TryCatchExample;



[TestClass]
public sealed class TryCatchExampleTests
{

    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "TryCatchExample";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);
        var content = new StringContent("", Encoding.UTF8, "text/plain");

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

        //Assert: Action calling function was successful
        var actionStatus = logicAppTestManager.GetActionStatus("HTTP_-_Get_from_Google");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Action calling function was successful
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
    }
    
    [Ignore("We can not simulate the error in this mode of testing")]
    [TestMethod]
    public void Google_Returns_An_Error()
    {
        //Arrange
        var workflowName = "TryCatchExample";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);
        var content = new StringContent("", Encoding.UTF8, "text/plain");

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

        //Assert: Action calling google got an error <-- 🚨 Note we can not simulate the error in this mode of testing
        var actionStatus = logicAppTestManager.GetActionStatus("HTTP_-_Get_from_Google");
        Assert.AreEqual(actionStatus, ActionStatus.Failed);

        //Assert: the normal response is skipped
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Skipped);

        //Assert: Action returning an error response is successful
        actionStatus = logicAppTestManager.GetActionStatus("Response_Error");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
    }
}
