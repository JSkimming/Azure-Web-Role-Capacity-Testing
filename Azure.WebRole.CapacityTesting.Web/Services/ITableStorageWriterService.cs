using System.Threading.Tasks;
using Azure.WebRole.CapacityTesting.Models;

namespace Azure.WebRole.CapacityTesting.Services
{
    public interface ITableStorageWriterService
    {
        CapacityTestData WriteToStorage();
        Task<CapacityTestData> WriteToStorageAsyncCtp();
        Task<CapacityTestData> WriteToStorageAsyncTpl();
    }
}
