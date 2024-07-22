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

        private readonly ILogger _logger;

        public FetchDataFunction(ILoggerFactory loggerFactory, IApiService apiService, ILoggingService loggingService, IBlobStorageService blobStorageService)
        {
            _logger = loggerFactory.CreateLogger<FetchDataFunction>();
            _apiService = apiService;
            _loggingService = loggingService;
            _blobStorageService = blobStorageService;
        }


        [FunctionName("FetchDataFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var result = await _apiService.FetchDataAsync();

            if (result.IsSuccess)
            {
                await _loggingService.LogAsync(result);
                await _blobStorageService.StorePayloadAsync(result.Payload);
            }
            else
            {
                await _loggingService.LogAsync(result);
            }

        }
    }
}
