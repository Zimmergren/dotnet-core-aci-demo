using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Zimmergren.ACI.Demo1
{
    public class Program
    {
        private static CloudTableClient _tableClient;
        private static CloudTable _table;

        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureStorageAccountConnectionString"));
            _tableClient = storageAccount.CreateCloudTableClient();
            _table = _tableClient.GetTableReference("acidemotable");
            _table.CreateIfNotExistsAsync().Wait();
            
            while (true)
            {
                UpdateAzureStorage();
            }
        }
        
        private static void UpdateAzureStorage()
        {
            SampleEntity entity = new SampleEntity
            {
                MachineName = Environment.MachineName,
                Message = $"I was processesed at {DateTime.UtcNow:u}"
            };

            TableOperation insertOperation = TableOperation.Insert(entity);
            _table.ExecuteAsync(insertOperation).Wait();

            string output = $"MESSAGE: {entity.Message}. MACHINE: {entity.MachineName}.";
            Console.WriteLine(output);
        }
    }

    public class SampleEntity : TableEntity
    {
        public SampleEntity()
        {
            this.RowKey = Guid.NewGuid().ToString("N");
            this.PartitionKey = "pk";
            this.ETag = "*";
        }

        public string MachineName { get; set; }
        public string Message { get; set; }
    }

}
