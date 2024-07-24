using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using ZuumTask1;


[assembly: FunctionsStartup(typeof(ListLogsFunction.Startup))]

namespace ListLogsFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
            string tableName = Environment.GetEnvironmentVariable("TableName");

            builder.Services.AddSingleton<ILoggingService, LoggingService>(loggingService =>
            {
                var tableStorageClient = new LogTableStorageClient(connectionString, tableName);
                var logService = new LoggingService(tableStorageClient);
                return logService;
            });
        }
    }
}
