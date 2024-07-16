using System;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Zuum_Task_1
{
    public class FetchDataFunction
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("Function1")]
        public async void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {

        }
    }
}
