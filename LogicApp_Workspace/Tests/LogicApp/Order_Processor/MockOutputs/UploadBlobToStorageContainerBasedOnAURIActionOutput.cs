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
    /// The <see cref="UploadBlobToStorageContainerBasedOnAURIActionMock"/> class.
    /// </summary>
    public class UploadBlobToStorageContainerBasedOnAURIActionMock : ActionMock
    {
        /// <summary>
        /// Creates a mocked instance for  <see cref="UploadBlobToStorageContainerBasedOnAURIActionMock"/> with static outputs.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionMock(TestWorkflowStatus status = TestWorkflowStatus.Succeeded, string name = null, UploadBlobToStorageContainerBasedOnAURIActionOutput outputs = null)
            : base(status: status, name: name, outputs: outputs ?? new UploadBlobToStorageContainerBasedOnAURIActionOutput())
        {
        }

        /// <summary>
        /// Creates a mocked instance for  <see cref="UploadBlobToStorageContainerBasedOnAURIActionMock"/> with static error info.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionMock(TestWorkflowStatus status, string name = null, TestErrorInfo error = null)
            : base(status: status, name: name, error: error)
        {
        }

        /// <summary>
        /// Creates a mocked instance for <see cref="UploadBlobToStorageContainerBasedOnAURIActionMock"/> with a callback function for dynamic outputs.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionMock(Func<TestExecutionContext, UploadBlobToStorageContainerBasedOnAURIActionMock> onGetActionMock, string name = null)
            : base(onGetActionMock: onGetActionMock, name: name)
        {
        }
    }


    /// <summary>
    /// Class for UploadBlobToStorageContainerBasedOnAURIActionOutput representing an object with properties.
    /// </summary>
    public class UploadBlobToStorageContainerBasedOnAURIActionOutput : MockOutput
    {
        public HttpStatusCode StatusCode {get; set;}

        /// <summary>
        /// The response from the upload blob action.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionOutputBody Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadBlobToStorageContainerBasedOnAURIActionOutput"/> class.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionOutput()
        {
            this.StatusCode = HttpStatusCode.OK;
            this.Body = new UploadBlobToStorageContainerBasedOnAURIActionOutputBody();
        }

    }

    /// <summary>
    /// The response from the upload blob action.
    /// </summary>
    public class UploadBlobToStorageContainerBasedOnAURIActionOutputBody
    {
        /// <summary>
        /// The blob properties.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionOutputBodyProperties Properties { get; set; }

        /// <summary>
        /// The blob metadata.
        /// </summary>
        public JObject Metadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadBlobToStorageContainerBasedOnAURIActionOutputBody"/> class.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionOutputBody()
        {
            this.Properties = new UploadBlobToStorageContainerBasedOnAURIActionOutputBodyProperties();
            this.Metadata = new JObject();
        }

    }

    /// <summary>
    /// The blob properties.
    /// </summary>
    public class UploadBlobToStorageContainerBasedOnAURIActionOutputBodyProperties
    {
        /// <summary>
        /// The creation time for the blob.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// The blob type.
        /// </summary>
        public string BlobType { get; set; }

        /// <summary>
        /// Blob full path with container name.
        /// </summary>
        public string BlobFullPathWithContainer { get; set; }

        /// <summary>
        /// The content disposition.
        /// </summary>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// The content MD5 hash.
        /// </summary>
        public string ContentMD5 { get; set; }

        /// <summary>
        /// The type of content.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The language of the content.
        /// </summary>
        public string ContentLanguage { get; set; }

        /// <summary>
        /// The ETag for the blob.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadBlobToStorageContainerBasedOnAURIActionOutputBodyProperties"/> class.
        /// </summary>
        public UploadBlobToStorageContainerBasedOnAURIActionOutputBodyProperties()
        {
            this.CreationTime = new DateTime();
            this.BlobType = string.Empty;
            this.BlobFullPathWithContainer = string.Empty;
            this.ContentDisposition = string.Empty;
            this.ContentMD5 = string.Empty;
            this.ContentType = string.Empty;
            this.ContentLanguage = string.Empty;
            this.ETag = string.Empty;
        }

    }

}