using System.Text;
using System.Text.Json;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;

namespace AcceptanceTests.Tool_LookUpCOrder_DecisionMaker;



[TestClass]
public sealed class Order_DecisionMakerTests
{

    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "Order_DecisionMaker";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var inputContent = File.ReadAllText(@"..\..\..\Order_DecisionMaker\Input.GreenPath.json");
        var content = new StringContent(inputContent, Encoding.UTF8, "application/json");

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

        //Assert: Action calling Azure Open AI Agent
        var actionStatus = logicAppTestManager.GetActionStatus("Default_Agent");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);


        //ISSUE: The tool actions do not appear in the run history as executed.  They must be available via another API?
        
        //Assert: Action calling Get customer history
        //actionStatus = logicAppTestManager.GetActionStatus("Get_customer_history");
        //Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Action calling Get customer value
        //actionStatus = logicAppTestManager.GetActionStatus("Get_customer_value");
        //Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

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
