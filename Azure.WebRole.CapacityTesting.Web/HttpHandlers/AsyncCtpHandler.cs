using System;
using System.Web;
using Azure.WebRole.CapacityTesting.Models;
using Azure.WebRole.CapacityTesting.Services;

namespace Azure.WebRole.CapacityTesting.HttpHandlers
{
    public class AsyncCtpHandler : IHttpAsyncHandler
    {
        private readonly ITableStorageWriterService _tsService;

        public AsyncCtpHandler()
            : this(new TableStorageWriterService())
        { }

        public AsyncCtpHandler(ITableStorageWriterService tsService)
        {
            if (tsService == null) throw new ArgumentNullException("tsService");
            _tsService = tsService;
        }

        #region Implementation of IHttpHandler

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param>
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException("Synchronous ProcessRequest has not been implemented, the asynchronous equivalent must be called.");
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

        #endregion Implementation of IHttpHandler

        #region Implementation of IHttpAsyncHandler

        async void ProcessRequestAsync(HttpContext context, AsyncCallback cb, SimpleAsyncResult<CapacityTestData> asyncResult)
        {
            try
            {
                CapacityTestData data = await _tsService.WriteToStorageAsyncCtp();

                SyncHandler.WriteResultsToResponse(context.Response, data);

                asyncResult.Result = data;
            }
            catch (Exception ex)
            {
                asyncResult.Exception = ex;
            }

            if (cb != null) cb(asyncResult);
        }

        /// <summary>
        /// Initiates an asynchronous call to the HTTP handler.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process.
        /// </returns>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests. </param><param name="cb">The <see cref="T:System.AsyncCallback"/> to call when the asynchronous method call is complete. If <paramref name="cb"/> is null, the delegate is not called. </param><param name="extraData">Any extra data needed to process the request. </param>
        IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var asyncResult = new SimpleAsyncResult<CapacityTestData>(extraData);

            ProcessRequestAsync(context, cb, asyncResult);

            return asyncResult;
        }

        /// <summary>
        /// Provides an asynchronous process End method when the process ends.
        /// </summary>
        /// <param name="result">An <see cref="T:System.IAsyncResult"/> that contains information about the status of the process. </param>
        void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        {
            var asyncResult = (SimpleAsyncResult<CapacityTestData>) result;
            if (asyncResult.Exception != null)
                throw asyncResult.Exception;
        }

        #endregion Implementation of IHttpAsyncHandler
    }
}
