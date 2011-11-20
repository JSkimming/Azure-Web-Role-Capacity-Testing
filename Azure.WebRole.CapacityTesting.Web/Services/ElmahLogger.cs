using System;
using System.Diagnostics;
using System.Web;
using Elmah;

namespace Azure.WebRole.CapacityTesting.Services
{
    class ElmahLogger : ILogger
    {
        #region Implementation of ILogger

        void ILogger.LogException(Exception error)
        {
            // Nothing to do if there is no exception.
            if (error == null) return;

            try
            {
                if (HttpContext.Current != null)
                {
                    ErrorSignal
                        .FromCurrentContext()
                        .Raise(error);
                }
                else
                {
                    ErrorLog.GetDefault(null).Log(new Error(error));
                }
            }
            catch (Exception loggerException)
            {
                // If failed to log with ELMAH trace it.
                Trace.TraceError(error.ToString());
                Trace.TraceError(loggerException.ToString());
            }
        }

        #endregion  Implementation of ILogger
    }
}
