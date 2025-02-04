﻿using Moq;
using Moq.Protected;
using System.Net;

namespace ZuumTask1.Tests
{
    [TestClass]
    public class ApiServiceTests
    {
        private Mock<HttpMessageHandler>? _msgHandler;
        private ApiService _apiService;
        private HttpClient _httpClient;
        private string URL = "https://api.publicapis.org/random?auth=null";

        [TestInitialize]
        public void Initialize()
        {
            _msgHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_msgHandler.Object);
            _apiService = new ApiService(_httpClient);
        }


        [TestMethod]
        public async Task FetchDataAsync_ShouldReturnSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange
            var expectedContent = "some data";
            var mockedProtected = _msgHandler.Protected();
            

            var setupApiRequest = mockedProtected.Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                );

            var apiMockedResponse =
                setupApiRequest.ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedContent)
                });

            apiMockedResponse.Verifiable();

            // Act
            var response = await _apiService.FetchDataAsync(URL);

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(expectedContent, response.Payload);
            Assert.IsNull(response.ErrorMessage);
        }

        [TestMethod]
        public async Task FetchDataAsync_ShouldReturnFailure_WhenApiCallFails()
        {
            // Arrange
            var expectedErrorMessage = "Request failed";

            var mockedProtected = _msgHandler.Protected();

            var setupApiRequest = mockedProtected.Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());

            var apiMockedResponse = setupApiRequest.ThrowsAsync(new HttpRequestException(expectedErrorMessage));

            // Act
            var response = await _apiService.FetchDataAsync(URL);

            // Assert
            Assert.IsFalse(response.IsSuccess);
            Assert.IsNull(response.Payload);
            Assert.AreEqual(expectedErrorMessage, response.ErrorMessage);
        }
    }
}