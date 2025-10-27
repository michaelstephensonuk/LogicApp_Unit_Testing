using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;

namespace LogicApp.Tests.Mocks.Order_DecisionMaker
{
    /// <summary>
    /// The <see cref="GetCustomerHistoryActionMock"/> class.
    /// </summary>
    public class GetCustomerHistoryActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="GetCustomerHistoryActionMock"/> with static outputs.
        /// </summary>
        public GetCustomerHistoryActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, GetCustomerHistoryActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new GetCustomerHistoryActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="GetCustomerHistoryActionMock"/> with static error info.
        /// </summary>
        public GetCustomerHistoryActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="GetCustomerHistoryActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public GetCustomerHistoryActionMock(Func<TestExecutionContext, GetCustomerHistoryActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for GetCustomerHistoryActionOutput representing an object with properties.
    /// </summary>
    public class GetCustomerHistoryActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCustomerHistoryActionOutput"/> class.
        /// </summary>
        public GetCustomerHistoryActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = "";
        }

    }

}