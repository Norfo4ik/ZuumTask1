﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Azure.Data.Tables;
using Azure;
using Azure.Storage.Blobs;
using System.IO;
using Azure.Storage.Blobs.Models;
using System.Reflection.Metadata;

namespace ZuumTask1
{

    public class PayloadBlobStorageClient : IPayloadBlobStorageClient
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

        public async Task<ApiResponse> FetchDataAsync(string URL)
        {
            var responce = new ApiResponse();

            try
            {
                var result = await _httpClient.GetStringAsync(URL);
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

        public async Task LogAsync(ApiResponse response, string GUID) 
        {
            var log = new LogEntity
            {
                PartitionKey = "Log",
                RowKey = GUID,
                Timestamp = DateTime.UtcNow,
                IsSuccess = response.IsSuccess,
                ErrorMessage = response.ErrorMessage
            };

            await _tableClient.AddEntityAsync(log);        
        }

        public async Task<List<LogEntity>> GetLogsAsync(string from, string to) 
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

        public BlobStorageService(IPayloadBlobStorageClient payloadBlobStorageClient) 
        {
            _containerClient = payloadBlobStorageClient.GetContainerClient();   
        }

        public async Task StorePayloadAsync(string payload, string GUID)
        {
            string blobName = GUID;
            string filePath = Path.Combine(Path.GetTempPath(), blobName);

            File.WriteAllText(filePath, payload);            

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(filePath);

            File.Delete(filePath);
        }


        public async Task<string> GetPayloadAsync(string Id)
        {
            string data = String.Empty;
            BlobClient blobClient = _containerClient.GetBlobClient(Id);
            if (await blobClient.ExistsAsync())
            {
                var download = await blobClient.DownloadAsync();
                using var streamReader = new StreamReader(download.Value.Content);
                data = await streamReader.ReadToEndAsync();
            }
            
            return data;
        }
    }
}