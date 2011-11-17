using System;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Azure.WebRole.CapacityTesting.Models;
using Azure.WebRole.CapacityTesting.Properties;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Azure.WebRole.CapacityTesting.AsyncCtp;

namespace Azure.WebRole.CapacityTesting.Controllers
{
    public class HomeController : Controller
    {
        private const string TableName = "CapacityTesting";
        private const string StorageAccountSetting = "StorageAccount";
        //
        // GET: /Home/

        public ActionResult Index()
        {
            var sw = new Stopwatch();
            sw.Start();

            var data =
                new CapacityTestData
                    {
                        PartitionKey = "Synchronous",
                        RowKey       = Guid.NewGuid().ToString().ToUpper(),
                        RequestStart = DateTime.UtcNow,
                    };

            CloudTableClient cloudTableClient = GetCloudTableClient();
            cloudTableClient.CreateTableIfNotExist(TableName);

            TableServiceContext context = cloudTableClient.GetDataServiceContext();

            context.AddObject(TableName, data);
            DataServiceResponse response1 = context.SaveChangesWithRetries();

            sw.Stop();
            data.ElapsedTicks = sw.ElapsedTicks;
            data.RequestEnd   = DateTime.UtcNow;

            context.UpdateObject(data);
            DataServiceResponse response2 = context.SaveChangesWithRetries();

            return View(data);
        }

        private static CloudTableClient GetCloudTableClient()
        {
            // If the instance us runing in Azure, then use the configuration setting.
            if(!RoleEnvironment.IsAvailable)
            {
                return CloudStorageAccount.FromConfigurationSetting(StorageAccountSetting).CreateCloudTableClient();
            }

            // As a fall-back get the connection string from the web.config.
            return CloudStorageAccount.Parse(Settings.Default.StorageAccount).CreateCloudTableClient();
        }

        public async Task<ActionResult> Async()
        {
            var sw = new Stopwatch();
            sw.Start();

            var data =
                new CapacityTestData
                    {
                        PartitionKey = "Asynchronous",
                        RowKey       = Guid.NewGuid().ToString().ToUpper(),
                        RequestStart = DateTime.UtcNow,
                    };

            CloudTableClient cloudTableClient = GetCloudTableClient();
            await cloudTableClient.CreateTableIfNotExistAsync(TableName);

            TableServiceContext context = cloudTableClient.GetDataServiceContext();

            context.AddObject(TableName, data);
            DataServiceResponse response1 = await context.SaveChangesWithRetriesAsync();

            sw.Stop();
            data.ElapsedTicks = sw.ElapsedTicks;
            data.RequestEnd   = DateTime.UtcNow;

            context.UpdateObject(data);
            DataServiceResponse response2 = await context.SaveChangesWithRetriesAsync();

            return View(data);
        }
    }
}
