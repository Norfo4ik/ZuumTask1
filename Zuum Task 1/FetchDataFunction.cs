using System.IO;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Zuum_Task_1
{
    public class FetchDataFunction
    {
        static HttpClient httpClient = new HttpClient();

        [FunctionName("Function1")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:ConnectionString");
            string containerName = Environment.GetEnvironmentVariable("ConnectionStrings:ContainerName");

            PayloadBlobStorageClient payloadBlobStorage = new PayloadBlobStorageClient(connectionString, containerName);

            BlobStorageService blobStorageService = new BlobStorageService(payloadBlobStorage);

            List<string> blobs = new List<string>();

            blobs = await blobStorageService.ListBlobsAsync();

            blobStorageService.GetPayloadAsync(blobs);

        }
    }
}
