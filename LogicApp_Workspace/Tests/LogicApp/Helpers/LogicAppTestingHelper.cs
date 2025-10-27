using System;
using System.Linq;
using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogicApp.Helpers;

public class LogicAppTestingHelper
{

    public static void LogTestRunInfo(TestContext testContext, TestWorkflowRun testRun)
    {
        //Log Workflow Results for troubleshooting
            if (testRun.Error != null)
            {
                testContext.WriteLine($"Error Code: {testRun.Error.Code}, Message: {testRun.Error.Message}");
                if (testRun.Error.Details != null)
                {
                    foreach (var e in testRun.Error.Details)
                        testContext.WriteLine(e.Message);
                }
            }

            testContext.WriteLine("Workflow Action Details:");
            testRun.Actions.ToList().ForEach(action =>
            {
                testContext.WriteLine($"Action: {action.Key}, Status: {action.Value.Status}");
                if (action.Value.Error != null)
                {
                    testContext.WriteLine($"Error Code: {action.Value.Error.Code}, Message: {action.Value.Error.Message}");
                }
            });
    }
}
