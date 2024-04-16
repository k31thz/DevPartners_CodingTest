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
        // Declare fields 
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly RainfallController _controller;

        // Initialize fields in the constructor
        public RainfallControllerTests()
        {
            _clientFactoryMock = new Mock<IHttpClientFactory>();
            _controller = new RainfallController(_clientFactoryMock.Object);
        }

        [Fact]
        public async Task HappyPath()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()  //Setup the protected sendasync method to return a successful response
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
            // Setup mock httpclient mimicing the controller logic
            var client = new HttpClient(mockHttpMessageHandler.Object);
            _clientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act on the mocked httpclient
            var result = await _controller.GetReadings("3680");
            // Assert
            // Check that the result is an OkObjectResult
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("wrongId", HttpStatusCode.BadRequest)]
        [InlineData(null, HttpStatusCode.BadRequest)]
        public async Task UnhappyPath(string stationId, HttpStatusCode statusCode)
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()     // Setup the protected SendAsync method to return a response with the specified status code
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                });

            // Setup mock httpclient mimicing the controller logic
            var client = new HttpClient(mockHttpMessageHandler.Object);
            _clientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act on method with a wrong parameter
            var result = await _controller.GetReadings(stationId);

            // Assert
            //Check that the result is a BadRequest result
            Assert.IsType<BadRequestResult>(result);
        }
    }
}
