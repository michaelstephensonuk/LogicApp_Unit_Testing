using System.Text;
using System.Text.Json;
using IPB.LogicApp.Standard.Testing;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunActionDetails;
using IPB.LogicApp.Standard.Testing.Model.WorkflowRunOverview;
using Newtonsoft.Json.Linq;

namespace AcceptanceTests.Order_Processor;



[TestClass]
public sealed class Order_ProcessorTests
{

    [TestMethod]
    public void GreenPath()
    {
        //Arrange
        var workflowName = "Order_Processor";
        var logicAppTestManager = LogicAppTestManagerBuilder.Build(workflowName);

        var inputContent = File.ReadAllText(@"..\..\..\Order_Processor\Input.GreenPath.json");
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

        //Assert: Action calling child workflow which encapsulates the agent
        var actionStatus = logicAppTestManager.GetActionStatus("Agent_Process_Order");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        // Get the message returned by the agent & convert to json so we can use the message body
        var agentReponse = logicAppTestManager.GetActionOutputMessage("Agent_Process_Order");
        Assert.IsNotNull(agentReponse, "Agent response should not be null");

        var agentResponseActionJson = JObject.Parse(agentReponse);
        var agentResponseBody = agentResponseActionJson["body"];
        var agentResponseBodyJson = JObject.Parse(agentResponseBody.ToString());
        var isVip = (bool)agentResponseBodyJson["is_vip"];

        //CHALLENGE: We dont always know what the agent will return so have to make some assumptions here
        if (isVip)
        {
            //Assert: DO Save the order to cosmos db
            actionStatus = logicAppTestManager.GetActionStatus("Create_or_update_item");
            Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

            //Assert: DO NOT save the order to blob storage
            actionStatus = logicAppTestManager.GetActionStatus("Upload_blob_to_storage_container_based_on_a_URI");
            Assert.AreEqual(actionStatus, ActionStatus.Skipped);
        }
        else
        {
            //Assert: DO Save the order to blob storage
            actionStatus = logicAppTestManager.GetActionStatus("Upload_blob_to_storage_container_based_on_a_URI");
            Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

            //Assert: DO NOT Save the order to cosmos db
            actionStatus = logicAppTestManager.GetActionStatus("Create_or_update_item");
            Assert.AreEqual(actionStatus, ActionStatus.Skipped);
        }
        

        //Assert: Action calling function was successful
        actionStatus = logicAppTestManager.GetActionStatus("Response");
        Assert.AreEqual(actionStatus, ActionStatus.Succeeded);

        //Assert: Workflow run status is succeeded
        var workflowRunStatus = logicAppTestManager.GetWorkflowRunStatus();
        Assert.AreEqual(WorkflowRunStatus.Succeeded, workflowRunStatus);
    }
}
