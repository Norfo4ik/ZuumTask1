using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZuumTask1
{
    public class ListLogsFunction
    {
        private ILoggingService _loggingService;

        public ListLogsFunction(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [FunctionName("ListLogsFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string from = req.Query["from"];
            string to = req.Query["to"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            from = from ?? data?.From;
            to = to ?? data?.To;

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return new BadRequestObjectResult("Please provide 'from' and 'to' query parameters.");
            }

            List<LogEntity> logs = new List<LogEntity>();

            logs = await _loggingService.GetLogsAsync(from, to);

            return new OkObjectResult(logs);
        }
    }
}

