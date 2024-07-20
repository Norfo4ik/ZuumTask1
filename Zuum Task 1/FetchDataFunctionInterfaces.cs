using Azure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;

namespace Zuum_Task_1
{
    interface IApiService
    {
        Task<ApiResponse> FetchDataAsync();
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Payload { get; set; }
        public string ErrorMessage { get; set; }

    }

    interface ILoggingService
    {
        Task LogAsync(ApiResponse response);
        Task <IEnumerable<LogEntity>> GetLogsAsync(string from,  string to);
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

    interface IBlobStorageService
    {
        Task StorePayloadAsync(string payload);
        Task<string> GetPayloadAsync(string logId);
    }
}