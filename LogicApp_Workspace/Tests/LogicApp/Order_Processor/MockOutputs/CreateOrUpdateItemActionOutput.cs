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
    /// The <see cref="CreateOrUpdateItemActionMock"/> class.
    /// </summary>
    public class CreateOrUpdateItemActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="CreateOrUpdateItemActionMock"/> with static outputs.
        /// </summary>
        public CreateOrUpdateItemActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, CreateOrUpdateItemActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new CreateOrUpdateItemActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="CreateOrUpdateItemActionMock"/> with static error info.
        /// </summary>
        public CreateOrUpdateItemActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="CreateOrUpdateItemActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public CreateOrUpdateItemActionMock(Func<TestExecutionContext, CreateOrUpdateItemActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for CreateOrUpdateItemActionOutput representing an object with properties.
    /// </summary>
    public class CreateOrUpdateItemActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The response of the operation.
        /// </summary>
        public CreateOrUpdateItemActionOutputBody Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateItemActionOutput"/> class.
        /// </summary>
        public CreateOrUpdateItemActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new CreateOrUpdateItemActionOutputBody();
        }

    }

    /// <summary>
    /// The response of the operation.
    /// </summary>
    public class CreateOrUpdateItemActionOutputBody
    {
        /// <summary>
        /// The entity tag associated with the item.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// The last modified timestamp associated with the item.
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// The activity Id for the item request.
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// The Id associated with the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The content of the item.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The item request charge measured in request units.
        /// </summary>
        public string RequestCharge { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrUpdateItemActionOutputBody"/> class.
        /// </summary>
        public CreateOrUpdateItemActionOutputBody()
        {
            this.ETag = string.Empty;
            this.Timestamp = string.Empty;
            this.ActivityId = string.Empty;
            this.Id = string.Empty;
            this.Content = string.Empty;
            this.RequestCharge = string.Empty;
        }

    }

}