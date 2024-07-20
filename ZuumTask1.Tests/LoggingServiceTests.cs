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
using Microsoft.Extensions.Configuration;



namespace ZuumTask1.Tests
{
    [TestClass]
    public class LoggingServiceTests
    {
        private Mock<LoggingService> _loggingService;
        private Mock<LogTableStorageClient> _logTableStorageClient;
        private Mock<TableClient> _mockTableClient;

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<LoggingServiceTests>().Build();
            return config;
        }

        

        [TestMethod]
        public async Task LogAsync_ShouldAddLogEntity()
        {
            var config = InitConfiguration();

            string connectionStr = config.GetValue<string>("ConnectionString");
            string tableName = config.GetValue<string>("TableName");


            var configurationStringSectionMock = new Mock<IConfigurationSection>();
            var tableNameSectionMock = new Mock<IConfigurationSection>();
            var configurationMock = new Mock<IConfiguration>();

            configurationStringSectionMock
                .Setup(x => x.Value)
                .Returns(connectionStr);

            tableNameSectionMock
                .Setup(x => x.Value)
                .Returns(tableName);

            configurationMock
                .Setup(x => x.GetSection("ConnectionString"))
                .Returns(configurationStringSectionMock.Object);

            configurationMock
                .Setup(x => x.GetSection("TableName"))
                .Returns(tableNameSectionMock.Object);

            _logTableStorageClient = new Mock<LogTableStorageClient>(configurationMock.Object);

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