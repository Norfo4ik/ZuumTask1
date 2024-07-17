using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Zuum_Task_1
{
    public class FetchDataFunction
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("Function1")]
        public async void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //var connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
            //var tableName = "LoggingAttemptResults";
            //var logTableStorageClient = new LogTableStorageClient(connectionString, tableName);
            //var loggingService = new LoggingService(logTableStorageClient);

            //var testResponceSuccess = new ApiResponse()
            //{
            //    IsSuccess = true,
            //    ErrorMessage = "-",
            //    Payload = "Tetspayload"
            //};

            //var testResponceError = new ApiResponse()
            //{
            //    IsSuccess = false,
            //    ErrorMessage = "404 Not found",
            //    Payload = "-"
            //};

            //await loggingService.LogAsync(testResponceSuccess);
            //await loggingService.LogAsync(testResponceError);

            //var logs = await loggingService.GetLogsAsync("2024-07-17", "2024-07-17");
 
        }
    }
}
