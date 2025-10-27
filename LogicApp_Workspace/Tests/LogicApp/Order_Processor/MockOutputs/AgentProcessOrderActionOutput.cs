using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;

namespace LogicApp.Tests.Mocks.Order_Processor
{
    /// <summary>
    /// The <see cref="AgentProcessOrderActionMock"/> class.
    /// </summary>
    public class AgentProcessOrderActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="AgentProcessOrderActionMock"/> with static outputs.
        /// </summary>
        public AgentProcessOrderActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, AgentProcessOrderActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new AgentProcessOrderActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="AgentProcessOrderActionMock"/> with static error info.
        /// </summary>
        public AgentProcessOrderActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="AgentProcessOrderActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public AgentProcessOrderActionMock(Func<TestExecutionContext, AgentProcessOrderActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for AgentProcessOrderActionOutput representing an object with properties.
    /// </summary>
    public class AgentProcessOrderActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The body.
        /// </summary>
        public JObject Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentProcessOrderActionOutput"/> class.
        /// </summary>
        public AgentProcessOrderActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new JObject();
        }

    }

}