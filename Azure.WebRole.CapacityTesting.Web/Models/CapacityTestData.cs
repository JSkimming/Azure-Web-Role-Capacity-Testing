using System;
using Microsoft.WindowsAzure.StorageClient;

namespace Azure.WebRole.CapacityTesting.Models
{
    public class CapacityTestData : TableServiceEntity
    {
        public DateTime RequestStart { get; set; }
        public DateTime? RequestEnd { get; set; }
        public long ElapsedMilliseconds { get; set; }
    }
}
