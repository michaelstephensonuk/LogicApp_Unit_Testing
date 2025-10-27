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
    /// The <see cref="GetCustomerValueActionMock"/> class.
    /// </summary>
    public class GetCustomerValueActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="GetCustomerValueActionMock"/> with static outputs.
        /// </summary>
        public GetCustomerValueActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, GetCustomerValueActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new GetCustomerValueActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="GetCustomerValueActionMock"/> with static error info.
        /// </summary>
        public GetCustomerValueActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="GetCustomerValueActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public GetCustomerValueActionMock(Func<TestExecutionContext, GetCustomerValueActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for GetCustomerValueActionOutput representing an object with properties.
    /// </summary>
    public class GetCustomerValueActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The body.
        /// </summary>
        public JObject Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCustomerValueActionOutput"/> class.
        /// </summary>
        public GetCustomerValueActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new JObject();
        }

    }

}