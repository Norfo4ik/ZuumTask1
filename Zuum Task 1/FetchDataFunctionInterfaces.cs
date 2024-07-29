using Azure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;

namespace ZuumTask1
{
    public interface IApiService
    {
        Task<ApiResponse> FetchDataAsync(string URL);
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Payload { get; set; }
        public string ErrorMessage { get; set; }

    }

    public interface ILoggingService
    {
        Task LogAsync(ApiResponse response, string GUID);
        Task <List<LogEntity>> GetLogsAsync(string from,  string to);
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

    public interface IBlobStorageService
    {
        Task StorePayloadAsync(string payload, string GUID);
        Task<string> GetPayloadAsync(string ID);
    }

    public interface IPayloadBlobStorageClient
    {
        BlobContainerClient GetContainerClient();
    }
}