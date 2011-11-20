using System;
using System.Data.Services.Client;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.WebRole.CapacityTesting.AsyncCtp;
using Azure.WebRole.CapacityTesting.Models;
using Azure.WebRole.CapacityTesting.Properties;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;

namespace Azure.WebRole.CapacityTesting.Services
{
    public class TableStorageWriterService : ITableStorageWriterService
    {
        private const string TableName = "CapacityTesting";
        private const string StorageAccountSetting = "StorageAccount";

        private static CloudTableClient GetCloudTableClient()
        {
            // If the instance us runing in Azure, then use the configuration setting.
            if (RoleEnvironment.IsAvailable)
            {
                return CloudStorageAccount.FromConfigurationSetting(StorageAccountSetting).CreateCloudTableClient();
            }

            // As a fall-back get the connection string from the web.config.
            return CloudStorageAccount.Parse(Settings.Default.StorageAccount).CreateCloudTableClient();
        }

        #region Implementation of ITableStorageWriterService

        CapacityTestData ITableStorageWriterService.WriteToStorage()
        {
            var sw = new Stopwatch();
            sw.Start();

            var data =
                new CapacityTestData
                {
                    PartitionKey = "Synchronous",
                    RowKey = Guid.NewGuid().ToString().ToUpper(),
                    RequestStart = DateTime.UtcNow,
                };

            CloudTableClient cloudTableClient = GetCloudTableClient();
            cloudTableClient.CreateTableIfNotExist(TableName);

            TableServiceContext context = cloudTableClient.GetDataServiceContext();

            context.AddObject(TableName, data);
            DataServiceResponse response1 = context.SaveChangesWithRetries();

            sw.Stop();
            data.ElapsedTicks = sw.ElapsedTicks;
            data.RequestEnd = data.RequestStart + new TimeSpan(data.ElapsedTicks);

            context.UpdateObject(data);
            DataServiceResponse response2 = context.SaveChangesWithRetries();

            return data;
        }

        async Task<CapacityTestData> ITableStorageWriterService.WriteToStorageAsyncCtp()
        {
            var sw = new Stopwatch();
            sw.Start();

            var data =
                new CapacityTestData
                {
                    PartitionKey = "Asynchronous",
                    RowKey = Guid.NewGuid().ToString().ToUpper(),
                    RequestStart = DateTime.UtcNow,
                };

            CloudTableClient cloudTableClient = GetCloudTableClient();
            await cloudTableClient.CreateTableIfNotExistAsync(TableName);

            TableServiceContext context = cloudTableClient.GetDataServiceContext();

            context.AddObject(TableName, data);
            DataServiceResponse response1 = await context.SaveChangesWithRetriesAsync();

            sw.Stop();
            data.ElapsedTicks = sw.ElapsedTicks;
            data.RequestEnd = data.RequestStart + new TimeSpan(data.ElapsedTicks);

            context.UpdateObject(data);
            DataServiceResponse response2 = await context.SaveChangesWithRetriesAsync();

            return data;
        }

        Task<CapacityTestData> ITableStorageWriterService.WriteToStorageAsyncTpl()
        {
            var sw = new Stopwatch();
            sw.Start();

            var data =
                new CapacityTestData
                {
                    PartitionKey = "Asynchronous",
                    RowKey = Guid.NewGuid().ToString().ToUpper(),
                    RequestStart = DateTime.UtcNow,
                };

            CloudTableClient cloudTableClient = GetCloudTableClient();
            TableServiceContext context = cloudTableClient.GetDataServiceContext();

            var tcs = new TaskCompletionSource<CapacityTestData>();

            cloudTableClient.CreateTableIfNotExistAsync(TableName)
                .ContinueWith(task1 =>
                {
                    if(task1.Exception != null)
                    {
                        tcs.SetException(task1.Exception);
                        return;
                    }

                    context.AddObject(TableName, data);

                    context.SaveChangesWithRetriesAsync()
                        .ContinueWith(task2 =>
                        {
                            if (task1.Exception != null)
                            {
                                tcs.SetException(task1.Exception);
                                return;
                            }

                            sw.Stop();
                            data.ElapsedTicks = sw.ElapsedTicks;
                            data.RequestEnd   = data.RequestStart + new TimeSpan(data.ElapsedTicks);

                            context.UpdateObject(data);

                            context
                                .SaveChangesWithRetriesAsync()
                                .ContinueWith(task3 =>
                                {
                                    if(task1.Exception != null)
                                    {
                                        tcs.SetException(task1.Exception);
                                        return;
                                    }

                                    tcs.SetResult(data);
                                });
                        });
                });

            return tcs.Task;
        }

        #endregion
    }
}
