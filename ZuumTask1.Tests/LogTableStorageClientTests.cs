using Zuum_Task_1;
using Microsoft.Extensions.Configuration;
using Moq;


namespace ZuumTask1.Tests
{
    [TestClass]
    public class LogTableStorageClientTests
    {
        private LogTableStorageClient _logTableStorageClient;
        

        [TestInitialize]
        public void Setup()
        {
            var configurationStringSectionMock = new Mock<IConfigurationSection>();
            var tableNameSectionMock = new Mock<IConfigurationSection>();
            var configurationMock = new Mock<IConfiguration>();

            configurationStringSectionMock
                .Setup(x => x.Value)
                .Returns("UseDevelopmentStorage=true");

            tableNameSectionMock
                .Setup(x => x.Value)
                .Returns("TestTable");

            configurationMock
                .Setup(x => x.GetSection("ConnectionString"))
                .Returns(configurationStringSectionMock.Object);

            configurationMock
                .Setup(x => x.GetSection("TableName"))
                .Returns(configurationStringSectionMock.Object);

            _logTableStorageClient = new LogTableStorageClient(configurationMock.Object);
        }

        [TestMethod]
        public void GetTableClient_ReturnsTableClient()
        {
            // Act
            var tableClient = _logTableStorageClient.GetTableClient();

            // Assert
            Assert.IsNotNull(tableClient);
        }
    }
}