using Microsoft.Azure.Workflows.UnitTesting.Definitions;
using Microsoft.Azure.Workflows.UnitTesting.ErrorResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System;

namespace LogicApp.Tests.Mocks.HelloWorld
{
    /// <summary>
    /// The <see cref="CallALocalFunctionInThisLogicAppActionMock"/> class.
    /// </summary>
    public class CallALocalFunctionInThisLogicAppActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="CallALocalFunctionInThisLogicAppActionMock"/> with static outputs.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, CallALocalFunctionInThisLogicAppActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new CallALocalFunctionInThisLogicAppActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="CallALocalFunctionInThisLogicAppActionMock"/> with static error info.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="CallALocalFunctionInThisLogicAppActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionMock(Func<TestExecutionContext, CallALocalFunctionInThisLogicAppActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for CallALocalFunctionInThisLogicAppActionOutput representing an object with properties.
    /// </summary>
    public class CallALocalFunctionInThisLogicAppActionOutput : MockOutput
    {
        /// <summary>
        /// The function's output.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionOutputBody Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallALocalFunctionInThisLogicAppActionOutput"/> class.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionOutput()
        {
            this.Body = new CallALocalFunctionInThisLogicAppActionOutputBody();
        }

    }

    /// <summary>
    /// The function's output.
    /// </summary>
    public class CallALocalFunctionInThisLogicAppActionOutputBody
    {
        public int ZipCode { get; set; }

        public string CurrentWeather { get; set; }

        public string DayLow { get; set; }

        public string DayHigh { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallALocalFunctionInThisLogicAppActionOutputBody"/> class.
        /// </summary>
        public CallALocalFunctionInThisLogicAppActionOutputBody()
        {
            this.ZipCode = 0;
            this.CurrentWeather = string.Empty;
            this.DayLow = string.Empty;
            this.DayHigh = string.Empty;
        }

    }

}