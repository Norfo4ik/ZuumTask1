using Azure;
using Azure.Data.Tables;
using Zuum_Task_1;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace ZuumTask1.Tests
{
    [TestClass]
    public class LoggingServiceTests
    {
        private Mock<TableClient> _tableClientMock;
        private LoggingService _loggingService;

        [TestInitialize]
        public void Setup()
        {
            _tableClientMock = new Mock<TableClient>();
            var logTableStorageClientMock = new Mock<LogTableStorageClient>();
            logTableStorageClientMock.Setup(client => client.GetTableClient())
                                      .Returns(_tableClientMock.Object);

            _loggingService = new LoggingService(logTableStorageClientMock.Object);
        }

        [TestMethod]
        public async Task LogAsync_ShouldAddLogEntity()
        {
            // Arrange
            var response = new ApiResponse
            {
                IsSuccess = true,
                ErrorMessage = null
            };

            // Act
            await _loggingService.LogAsync(response);

            // Assert
            _tableClientMock.Verify(client => client.AddEntityAsync(
                It.Is<LogEntity>(log => log.IsSuccess == response.IsSuccess && log.ErrorMessage == response.ErrorMessage)),
                Times.Once);
        }

        [TestMethod]
        public async Task GetLogsAsync_ShouldReturnLogsInRange()
        {
            // Arrange
            var from = "2023-01-01T00:00:00Z";
            var to = "2023-12-31T23:59:59Z";
            var logs = new List<LogEntity>
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

            var pageableResponseMock = new Mock<Pageable<LogEntity>>();
            pageableResponseMock.Setup(m => m.GetEnumerator()).Returns(logs.GetEnumerator());

            _tableClientMock.Setup(client => client.Query(
                It.IsAny<Expression<Func<LogEntity, bool>>>(),
                null,
                null,
                null))
                .Returns(pageableResponseMock.Object);

            // Act
            var result = await _loggingService.GetLogsAsync(from, to);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("1", result.First().Id);
            Assert.AreEqual("2", result.Last().Id);
        }
    }

}