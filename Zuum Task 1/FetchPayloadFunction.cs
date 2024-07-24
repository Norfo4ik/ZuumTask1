using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ZuumTask1
{
    public class FetchPayloadFunction
    {
        private readonly IBlobStorageService _blobStorageService;

        public FetchPayloadFunction(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }
        [FunctionName("FetchPayloadFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "payload/{logId}")] HttpRequest req, string logId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var payload = await _blobStorageService.GetPayloadAsync(logId);

            if (string.IsNullOrEmpty(payload))
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(payload);
        }
    }
}
