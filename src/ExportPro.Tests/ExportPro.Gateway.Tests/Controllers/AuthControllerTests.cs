using System.Net;
using ExportPro.AuthService.Commands;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Library;
using ExportPro.Gateway.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
namespace ExportPro.Gateway.Tests;

[TestFixture]
public class AuthControllerTests
{
    private IMediator _mediator;
    private AuthController _controller;
    private IResponseCookies _mockCookies;
    private HttpResponse _mockHttpResponse;
    private AuthResponseDto authResponse;
    private UserRegisterDto _userRegisterDto;
    private UserLoginDto _loginUserDto;

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
        var mockRequestCookies = Substitute.For<IRequestCookieCollection>();
        mockRequestCookies.TryGetValue("refreshToken", out Arg.Any<string>())
                          .Returns(x => { x[1] = "refreshToken"; return true; });
        httpContext.Request.Cookies.Returns(mockRequestCookies);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        _userRegisterDto = new UserRegisterDto
        {
            Username = "testuser",
            Email = "TestUser2@example.com",
            Password = "password123"

        };
        authResponse = new AuthResponseDto
        {
            RefreshToken = "mockRefreshToken",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            Token = "mockToken",
            Username = "testuser"
        };
        _loginUserDto = new UserLoginDto
        {
            Email = "TestUser2@example.com",
            Password = "TestUser2@example.com"
        };

    }
    [Test]
    public async Task Register_WhenUserIsValid_ReturnsOkAndSetsRefreshTokenCookie()
    {
        //Arrange
        BaseResponse<AuthResponseDto> response = new()
        {
            IsSuccess = true,
            Messages = ["User registered"],
            ApiState = HttpStatusCode.OK,
            Data = authResponse
        };

        _mediator.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        //Act
        var result = await _controller.Register(_userRegisterDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        Assert.That(objectResult?.Value, Is.EqualTo(authResponse));
        _mockCookies.Received(1).Append("refreshToken", "mockRefreshToken", Arg.Is<CookieOptions>(options =>
            options.HttpOnly == true &&
            options.Secure == true &&
            options.Expires == authResponse.ExpiresAt
        ));
    }
    [Test]
    public async Task Register_WhenUserIsNotValid_ReturnsBadRequest()
    {
        //Arrange
        BaseResponse<AuthResponseDto> response = new()
        {
            IsSuccess = false,
            Messages = ["User failed to register"],
            ApiState = HttpStatusCode.BadRequest,
            Data = null
        };
        _mediator.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));

        //Act
        var result = await _controller.Register(_userRegisterDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(400));
        Assert.That(objectResult?.Value, Is.EqualTo(response.Messages));
    }
    [Test]
    public async Task Login_WhenUserIsValid_ReturnsOkAndSetsRefreshTokenCookie()
    {
        //Arrange
        var response = new BaseResponse<AuthResponseDto>
        {
            IsSuccess = true,
            Messages = ["Login successful"],
            ApiState = HttpStatusCode.OK,
            Data = authResponse
        };

        _mediator.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(response));

        //Act
        var result = await _controller.Login(_loginUserDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        Assert.That(objectResult?.Value, Is.EqualTo(authResponse));

        _mockCookies.Received(1).Append("refreshToken", "mockRefreshToken", Arg.Is<CookieOptions>(options =>
            options.HttpOnly == true &&
            options.Secure == true &&
            options.Expires == authResponse.ExpiresAt
        ));
    }
    [Test]
    public async Task Login_WhenUserIsNotValid_ReturnsUnauthorized()
    {
        //Arrange
        var response = new BaseResponse<AuthResponseDto>
        {
            IsSuccess = false,
            Messages = ["Login Failed"],
            ApiState = HttpStatusCode.Unauthorized,
            Data = null
        };

        _mediator.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(response));

        //Act
        var result = await _controller.Login(_loginUserDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(401));
        Assert.That(objectResult?.Value, Is.EqualTo(response.Messages));
    }
    [Test]
    public async Task Logout_WhenRefreshTokenExists_ReturnsOkAndDeletesRefreshTokens()
    {
        //Act
        var result = await _controller.Logout();
        var objectResult = result as ObjectResult;
       
        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        Assert.That(objectResult?.Value, Is.EqualTo("Logged out successfully."));
        await _mediator.Received(1).Send(Arg.Any<LogoutCommand>(), Arg.Any<CancellationToken>());
        _mockCookies.Received(1).Delete("refreshToken");
    }
}