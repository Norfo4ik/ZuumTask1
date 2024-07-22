using Azure;
using Azure.Data.Tables;
using Moq;

namespace ZuumTask1.Tests
{
    [TestClass]
    public class LoggingServiceTests
    {
        private Mock<LoggingService> _loggingService;
        private Mock<LogTableStorageClient> _logTableStorageClient;
        private Mock<TableClient> _mockTableClient;

        [TestMethod]
        public async Task LogAsync_ShouldAddLogEntity()
        {

            _logTableStorageClient = new Mock<LogTableStorageClient>("UseDevelopmentStorage=true;", "LoggingAttemptResults");

            _loggingService = new Mock<LoggingService>(_logTableStorageClient.Object);

            // Arrange
            var response = new ApiResponse
            {
                IsSuccess = true,
                ErrorMessage = null
            };


            // Act
            await _loggingService.Object.LogAsync(response);

            // Assert
            _loggingService.VerifyAll();

        }

        [TestMethod]
        public async Task GetLogsAsync_ShouldReturnLogsInRange()
        {
            // Arrange
            string from = "2023-01-01T00:00:00Z";
            string to = "2023-12-31T23:59:59Z";


            _mockTableClient = new Mock<TableClient>();

            var mockLogEntities = new List<LogEntity>
            {
                new LogEntity
                {
                    PartitionKey = "Log",
                    RowKey = "1",
                    Timestamp = DateTime.Parse("2023-06-01T00:00:00Z"),
                    IsSuccess = true,
                    ErrorMessage = null
                },
                new LogEntity
                {
                    PartitionKey = "Log",
                    RowKey = "2",
                    Timestamp = DateTime.Parse("2023-07-01T00:00:00Z"),
                    IsSuccess = false,
                    ErrorMessage = "Error"
                }
            }.AsQueryable();



            var page = Page<LogEntity>.FromValues(mockLogEntities.ToList(), default, new Mock<Response>().Object);

            var pageable = Pageable<LogEntity>.FromPages(new[] { page });
                        
            _mockTableClient.Setup(client => client.Query<LogEntity>(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
                .Returns(pageable);


            _loggingService = new Mock<LoggingService>(_mockTableClient.Object);



            // Act
            var result = await _loggingService.Object.GetLogsAsync(from, to);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("1", result.First().RowKey);
            Assert.AreEqual("2", result.Last().RowKey);
        }
    }

}