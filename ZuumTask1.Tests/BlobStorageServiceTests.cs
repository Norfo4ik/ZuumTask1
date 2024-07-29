using Moq;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;
using Azure;
using ZuumTask1;

namespace BlobStorageServiceTests
{
    [TestClass]
    public class BlobStorageServiceTests
    {
        private Mock<BlobContainerClient> _mockContainerClient;
        private Mock<BlobClient> _mockBlobClient;
        private BlobStorageService _blobStorageService;
        private Mock<IPayloadBlobStorageClient> _mockPayloadBlobStorageClient;

        [TestInitialize]
        public void Setup()
        {
            _mockContainerClient = new Mock<BlobContainerClient>();
            _mockBlobClient = new Mock<BlobClient>();
            _mockPayloadBlobStorageClient = new Mock<IPayloadBlobStorageClient>();

            _mockContainerClient.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(_mockBlobClient.Object);
            _mockPayloadBlobStorageClient.Setup(x => x.GetContainerClient()).Returns(_mockContainerClient.Object);

            _blobStorageService = new BlobStorageService(_mockPayloadBlobStorageClient.Object);
        }

        [TestMethod]
        public async Task StorePayloadAsync_ShouldStorePayload()
        {
            var payload = "Test Payload";
            var guid = Guid.NewGuid().ToString();
            var tempFilePath = Path.Combine(Path.GetTempPath(), guid);

            await _blobStorageService.StorePayloadAsync(payload, guid);

            Assert.IsFalse(tempFilePath.Equals(payload));
        }

        [TestMethod]
        public async Task GetPayloadAsync_ShouldReturnPayload_WhenBlobExists()
        {
            var id = Guid.NewGuid().ToString();
            var payload = "Test Payload";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            var blobDownloadInfo = BlobsModelFactory.BlobDownloadInfo(content: memoryStream);

            _mockBlobClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(true, null));
            _mockBlobClient.Setup(x => x.DownloadAsync(default)).ReturnsAsync(Response.FromValue(blobDownloadInfo, null));

            var result = await _blobStorageService.GetPayloadAsync(id);

            Assert.AreEqual(payload, result);
        }

        [TestMethod]
        public async Task GetPayloadAsync_ShouldReturnEmptyString_WhenBlobDoesNotExist()
        {
            var id = Guid.NewGuid().ToString();

            _mockBlobClient.Setup(x => x.ExistsAsync(default)).ReturnsAsync(Response.FromValue(false, null));

            var result = await _blobStorageService.GetPayloadAsync(id);

            Assert.AreEqual(string.Empty, result);
        }
    }
}

