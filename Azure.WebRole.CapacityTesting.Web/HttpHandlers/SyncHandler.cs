using System;
using System.Web;
using Azure.WebRole.CapacityTesting.Models;
using Azure.WebRole.CapacityTesting.Services;

namespace Azure.WebRole.CapacityTesting.HttpHandlers
{
    class SyncHandler : IHttpHandler
    {
        private readonly ITableStorageWriterService _tsService;

        public SyncHandler()
            : this(new TableStorageWriterService())
        { }

        public SyncHandler(ITableStorageWriterService tsService)
        {
            if (tsService == null) throw new ArgumentNullException("tsService");
            _tsService = tsService;
        }

        internal static void WriteResultsToResponse(HttpResponse httpResponse, CapacityTestData data)
        {
            httpResponse.ContentType = "text/plain";
            httpResponse.Write(string.Format("Sync result {0}", data.ElapsedTicks));
        }

        #region Implementation of IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            CapacityTestData data = _tsService.WriteToStorage();

            WriteResultsToResponse(context.Response, data);
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        bool IHttpHandler.IsReusable
        {
            get { return true; }
        }

        #endregion
    }
}