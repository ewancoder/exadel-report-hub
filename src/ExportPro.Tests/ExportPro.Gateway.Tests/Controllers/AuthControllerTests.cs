using MediatR;
using ExportPro.Gateway.Controllers;
using NSubstitute;
using ExportPro.Common.Shared.DTOs;
using ExportPro.AuthService.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using ExportPro.Common.Shared.Library;
namespace ExportPro.Gateway.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private IMediator _mediator;
        private AuthController _controller;
        private IResponseCookies _mockCookies;
        private HttpResponse _mockHttpResponse;
        [SetUp]
        public void Setup()
        {
            _mockCookies = Substitute.For<IResponseCookies>();
            _mockHttpResponse = Substitute.For<HttpResponse>();
            _mockHttpResponse.Cookies.Returns(_mockCookies);
            _mediator = Substitute.For<IMediator>();
            _controller = new AuthController(_mediator);
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Response.Returns(_mockHttpResponse);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
        [Test]
        public async Task Register_WhenRegisteringUser_ReturnsOk()
        {
            //Arrange
            UserRegisterDto user = new() 
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };
            AuthResponseDto authResponse = new()
            {
                RefreshToken = "mockRefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Token = "mockToken",
                Username = "testuser"
            };
            BaseResponse<AuthResponseDto>  response = new()
            {
                IsSuccess = true,
                Messages = ["User registered"],
                ApiState = HttpStatusCode.OK,
                Data = authResponse
            };
            _mediator.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(response));
           
            //Act
            var result = await _controller.Register(user);
            var okResult = result as ObjectResult;

            //Assert
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            Assert.That(okResult?.Value, Is.EqualTo(authResponse));
            _mockCookies.Received(1).Append("refreshToken", "mockRefreshToken", Arg.Is<CookieOptions>(options =>
                options.HttpOnly == true &&
                options.Secure == true &&
                options.Expires == authResponse.ExpiresAt
            ));
        }
        [Test]
        public async Task Register_WhenRegisteringUser_ReturnsBadRequest()
        {
            //Arrange
            UserRegisterDto user = new()
            {
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password123"
            };
            AuthResponseDto authResponse = new()
            {
                RefreshToken = "mockRefreshToken",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Token = "mockToken",
                Username = "testuser"
            };
            BaseResponse<AuthResponseDto> response = new()
            {
                IsSuccess = false,
                Messages = ["User registered"],
                ApiState = HttpStatusCode.BadRequest,
                Data = authResponse
            };
            _mediator.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(response));

            //Act
            var result = await _controller.Register(user);
            var okResult = result as ObjectResult;

            //Assert
            Assert.That(okResult?.StatusCode, Is.EqualTo(400));
            Assert.That(okResult?.Value, Is.EqualTo(response.Messages));
        }
    }
}