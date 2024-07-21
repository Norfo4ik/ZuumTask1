using System.IO;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Zuum_Task_1
{
    public class FetchDataFunction
    {
        static HttpClient httpClient = new HttpClient();

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<FetchDataFunction>().Build();
            return config;
        }

        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            PayloadBlobStorageClient payloadBlobStorage = new PayloadBlobStorageClient(InitConfiguration());

            BlobStorageService blobStorageService = new BlobStorageService(payloadBlobStorage);

            blobStorageService.StorePayloadAsync("This is test payload");

        }
    }
}
