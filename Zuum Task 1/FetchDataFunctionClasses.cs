using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Data.Tables;
using System.Runtime.CompilerServices;
using Azure;

namespace Zuum_Task_1
{
    public class LogTableStorageClient
    {
        private readonly TableClient _tableClient;
        private readonly string connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private readonly string tableName = "LoggingAttemptResults";

        public LogTableStorageClient()
        {
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient(tableName);
        }

        public LogTableStorageClient(string _connectionString, string _tableName)
        {
            var serviceClient = new TableServiceClient(_connectionString);
            _tableClient = serviceClient.GetTableClient(_tableName);
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
}