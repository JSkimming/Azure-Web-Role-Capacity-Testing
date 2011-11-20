using System;
using System.Diagnostics;
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
                ErrorSignal
                    .FromCurrentContext()
                    .Raise(error);
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
