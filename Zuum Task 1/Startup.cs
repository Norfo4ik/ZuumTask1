using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(ZuumTask1.Startup))]

namespace ZuumTask1
{
    public class Startup : FunctionsStartup
    {
        private static HttpClient httpClient = new HttpClient();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            string tableName = Environment.GetEnvironmentVariable("TableName");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            builder.Services.AddSingleton<IApiService>(api => new ApiService(httpClient));
            builder.Services.AddSingleton<ILoggingService, LoggingService>(loggingService =>
            {
                var tableStorageClient = new LogTableStorageClient(connectionString, tableName);
                var logService = new LoggingService(tableStorageClient);
                return logService;
            });
            builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>(blobStorageService =>
            {
                var payloadBlobStorageClient = new PayloadBlobStorageClient(connectionString, containerName);
                var blobStorService = new BlobStorageService(payloadBlobStorageClient);
                return blobStorService;
            });
        }
    }
}

