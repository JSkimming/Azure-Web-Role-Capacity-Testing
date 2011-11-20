using System;

namespace Azure.WebRole.CapacityTesting.Services
{
    public interface ILogger
    {
        void LogException(Exception ex);
    }
}
