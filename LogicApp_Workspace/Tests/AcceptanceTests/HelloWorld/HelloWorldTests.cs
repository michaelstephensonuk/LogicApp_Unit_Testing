using System.Text;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.HelloWorld;



[TestClass]
public sealed class HelloWorldTests
{
    //We add this category to run a subset of tests in the pipeline when we only want to 
    //run tests which verify the deployment of the Logic App vs running all tests which 
    //may be to invasive or require external dependencies.
    [TestCategory("DeploymentVerification")]    
    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "HelloWorld";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);
        var content = new StringContent("", Encoding.UTF8, "text/plain");

        //Act
        var response = logicAppTestManager.TriggerLogicAppWithPost(content, triggerName: "When_a_HTTP_request_is_received");

        //Assert: Successful response
        Assert.IsNotNull(response);
        Assert.AreEqual(200, (int)response.HttpResponse.StatusCode);

        //Assert: Response content contains expected data
        var responseContent = response.HttpResponse.Content.ReadAsStringAsync().Result;
        Assert.IsTrue(responseContent.Contains("ZipCode"));

        //Assert: Workflow run ID is not null
        Assert.IsNotNull(response.WorkFlowRunId);

        //Load Run History for more assertions
        logicAppTestManager.LoadWorkflowRunHistory();

        //Assert: Trigger was successful
        var triggerStatus = logicAppTestManager.GetTriggerStatus();
        Assert.AreEqual(triggerStatus, TriggerStatus.Succeeded);

        //Assert: Action calling function was successful
        var actionStatus = logicAppTestManager.GetActionStatus("Call a local function in this logic app");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
    }
}
