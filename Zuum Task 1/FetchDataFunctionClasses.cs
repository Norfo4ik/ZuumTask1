using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Data.Tables;
using Azure;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;

namespace ZuumTask1
{

    public class PayloadBlobStorageClient
    {
        private readonly BlobContainerClient _containerClient;

        public PayloadBlobStorageClient(string connectionString, string containerName)
        {

            var serviceClient = new BlobServiceClient(connectionString);

            _containerClient = serviceClient.GetBlobContainerClient(containerName);

        }

        public BlobContainerClient GetContainerClient()
        {
            return _containerClient;
        }
    }

    public class LogTableStorageClient
    {
        private readonly TableClient _tableClient;

        public LogTableStorageClient(string connectionString, string tableName)
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
        }

        public TableClient GetTableClient()
        {
            return _tableClient;
        }
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse> FetchDataAsync()
        {
            var responce = new ApiResponse();

            try
            {
                var result = await _httpClient.GetStringAsync("https://api.publicapis.org/random?auth=null");
                responce.IsSuccess = true;
                responce.Payload = result;
            }
            catch (Exception ex) 
            { 
                responce.IsSuccess = false;
                responce.ErrorMessage = ex.Message;
            }

            return responce;
        }
    }

    public class LoggingService : ILoggingService 
    {
        private readonly TableClient _tableClient;

        public LoggingService(TableClient client)
        {
            _tableClient = client;
        }

        public LoggingService(LogTableStorageClient logTableStorageClient) 
        {        
            _tableClient = logTableStorageClient.GetTableClient();
        }

        public async Task LogAsync(ApiResponse response) 
        {
            var log = new LogEntity
            {
                PartitionKey = "Log",
                RowKey = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow,
                IsSuccess = response.IsSuccess,
                ErrorMessage = response.ErrorMessage
            };

            await _tableClient.AddEntityAsync(log);        
        }

        public async Task<IEnumerable<LogEntity>> GetLogsAsync(string from, string to) 
        {             
            var fromTimestamp = DateTime.Parse(from);
            var toTimestamp = DateTime.Parse(to);

            Pageable<LogEntity> logs = _tableClient.Query<LogEntity>(filter: $"Timestamp gt '{fromTimestamp:O}' and Timestamp lt '{toTimestamp:O}'");

             return logs.Select(log => new LogEntity
             {                
                PartitionKey = log.PartitionKey,
                RowKey = log.RowKey,
                Timestamp = log.Timestamp,
                IsSuccess = log.IsSuccess,
                ErrorMessage = log.ErrorMessage

            }).ToList();        
        }    
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(PayloadBlobStorageClient payloadBlobStorageClient) 
        {
            _containerClient = payloadBlobStorageClient.GetContainerClient();   
        }

        public async Task StorePayloadAsync(string payload)
        {
            string blobName = Guid.NewGuid().ToString() + ".txt";
            string filePath = Path.Combine(Path.GetTempPath(), blobName);

            File.WriteAllText(filePath, payload);            

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(filePath);
        }


        public async Task GetPayloadAsync(List<string> blobs)
        {
            foreach (var blob in blobs) 
            {
                string filePath = Path.Combine(Path.GetTempPath(), blob);
                BlobClient blobClient = _containerClient.GetBlobClient(blob);
                BlobDownloadInfo download = await blobClient.DownloadAsync();

                using (var fileStream = File.OpenWrite(filePath))
                {
                    await download.Content.CopyToAsync(fileStream);
                }
            }
        }

        public async Task<List<string>> ListBlobsAsync()
        {
            List<string> blobList = new List<string>();            

            await foreach (var blobItems in _containerClient.GetBlobsAsync())
            {
                blobList.Add(blobItems.Name);
            }

            return blobList;
        }
    }
}