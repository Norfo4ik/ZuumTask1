using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Data.Tables;
using Azure;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace Zuum_Task_1
{
    public class LogTableStorageClient
    {

        private readonly IConfiguration _configuration;
        private readonly TableClient _tableClient;

        public LogTableStorageClient()
        {

            string connectionString = 
                _configuration.GetValue<string>("ConnectionString");

            string tableName = 
                _configuration.GetValue<string>("TableName");

            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
        }

        public LogTableStorageClient(IConfiguration config)
        {

            _configuration = config;
            string connectionString = _configuration.GetValue<string>("ConnectionString");
            string tableName = _configuration.GetValue<string>("TableName");

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


    //public class BlobStorageService : IBlobStorageService
    //{
    //    private readonly BlobServiceClient _blobServiceClient;

    //    public BlobStorageService(BlobServiceClient blobServiceClient) 
    //    {
    //        _blobServiceClient = blobServiceClient;
    //    }
    //}
}