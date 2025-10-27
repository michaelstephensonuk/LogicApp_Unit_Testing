using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;

namespace LogicApp.Tests.Mocks.Tool_LookUpCustomerHistory
{
    /// <summary>
    /// The <see cref="GetSampleTestDataActionMock"/> class.
    /// </summary>
    public class GetSampleTestDataActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="GetSampleTestDataActionMock"/> with static outputs.
        /// </summary>
        public GetSampleTestDataActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, GetSampleTestDataActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new GetSampleTestDataActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="GetSampleTestDataActionMock"/> with static error info.
        /// </summary>
        public GetSampleTestDataActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="GetSampleTestDataActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public GetSampleTestDataActionMock(Func<TestExecutionContext, GetSampleTestDataActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for GetSampleTestDataActionOutput representing an object with properties.
    /// </summary>
    public class GetSampleTestDataActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The body.
        /// </summary>
        public JObject Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSampleTestDataActionOutput"/> class.
        /// </summary>
        public GetSampleTestDataActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new JObject();
        }

    }

}