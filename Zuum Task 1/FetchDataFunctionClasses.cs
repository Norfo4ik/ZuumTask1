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

    public class LogEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = ETag.All;
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
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

        public async Task<IEnumerable<LogEntry>> GetLogsAsync(string from, string to) 
        { 
        
            var fromTimestamp = DateTime.Parse(from);
            var toTimestamp = DateTime.Parse(to);
            var logs = _tableClient.Query<LogEntity>(log => log.Timestamp >= fromTimestamp && log.Timestamp <= toTimestamp);

             return logs.Select(log => new LogEntry 
            { 
            
                Id = log.RowKey,
                Timestamp = log.Timestamp,
                IsSuccess = log.IsSuccess,
                ErrorMessage = log.ErrorMessage

            }).ToList();
        
        }
    
    }


}
