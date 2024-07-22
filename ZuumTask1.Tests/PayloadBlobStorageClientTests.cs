namespace ZuumTask1.Tests
{
    [TestClass]
    public class PayloadBlobStorageClientTests
    {
        private PayloadBlobStorageClient _payloadBlobStorageClient;

        [TestInitialize]
        public void Setup()
        {
            _payloadBlobStorageClient = new PayloadBlobStorageClient("UseDevelopmentStorage=true;", "TestTable");
        }

        [TestMethod()]
        public void GetContainerClientTest()
        {
            var blobStorageClient = _payloadBlobStorageClient.GetContainerClient();

            Assert.IsNotNull(blobStorageClient);
        }
    }
}