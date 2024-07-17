using Azure.Data.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Zuum_Task_1;


namespace ZuumTask1.Tests
{
    [TestClass]
    public class LogTableStorageClientTests
    {
        private LogTableStorageClient _logTableStorageClient;

        [TestInitialize]
        public void Setup()
        {
            _logTableStorageClient = new LogTableStorageClient("UseDevelopmentStorage=true;", "TestTable");
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