using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace ZuumTask1
{
    public class FetchDataFunction
    {

        private readonly IApiService _apiService;
        private readonly ILoggingService _loggingService;
        private readonly IBlobStorageService _blobStorageService;

        private readonly string URL = Environment.GetEnvironmentVariable("URL");

        public FetchDataFunction(IApiService apiService, ILoggingService loggingService, IBlobStorageService blobStorageService)
        {
            _apiService = apiService;
            _loggingService = loggingService;
            _blobStorageService = blobStorageService;
        }


        [FunctionName("FetchDataFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            
            string GUID = Guid.NewGuid().ToString();
            var result = await _apiService.FetchDataAsync(URL);

            if (result.IsSuccess)
            {
                await _loggingService.LogAsync(result, GUID);
                await _blobStorageService.StorePayloadAsync(result.Payload, GUID);
            }
            else
            {
                await _loggingService.LogAsync(result, GUID);
            }

        }
    }
}
