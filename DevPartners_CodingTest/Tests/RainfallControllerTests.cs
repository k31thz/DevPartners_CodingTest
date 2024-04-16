using Xunit;
using Moq;
using Moq.Protected;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DevPartners_CodingTest.Controllers;
using DevPartners_CodingTest.Models;

namespace DevPartners_CodingTest.Tests
{
    public class RainfallControllerTests
    {
        private readonly ILogger<RainfallControllerTests> _logger;
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly RainfallController _controller;

        public RainfallControllerTests()
        {
            _clientFactoryMock = new Mock<IHttpClientFactory>();
            _controller = new RainfallController(_clientFactoryMock.Object);
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RainfallControllerTests>();
        }

        [Fact]
        public async Task HappyPath()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"items\": [{\"value\": 1.0}]}", Encoding.UTF8, "application/json")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _clientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _controller.GetReadings("3680");
            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("wrongId", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async Task UnhappyPath(string stationId, HttpStatusCode statusCode)
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            _clientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _controller.GetReadings(stationId);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
