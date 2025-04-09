using System.Net;
using ExportPro.Auth.CQRS.Commands;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Auth.ServiceHost.Controllers;
using ExportPro.Common.Shared.Library;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ExportPro.Auth.Tests.ServiceHost.Controllers;
[TestFixture]
public class AuthTests
{
    private IMediator _mediator;
    private AuthController _authController;
    private IResponseCookies _mockResponseCookies;
    private UserLoginDto _loginDto;
    private UserRegisterDto _registerDto;
    private AuthResponseDto _authResponseDto;
    private void SettingHttpContext()
    {
        _mockResponseCookies = Substitute.For<IResponseCookies>();
        var mockHttpResponse = Substitute.For<HttpResponse>();
        mockHttpResponse.Cookies.Returns(_mockResponseCookies);
        var mockHttpContext = Substitute.For<HttpContext>();
        mockHttpContext.Response.Returns(mockHttpResponse);
        var mockRequestCookies = Substitute.For<IRequestCookieCollection>();
        mockRequestCookies.TryGetValue("refreshToken", out Arg.Any<string>())
                          .Returns(x => { x[1] = "refreshToken"; return true; });
        mockHttpContext.Request.Cookies.Returns(mockRequestCookies);
        _authController.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext
        };
    }
    private void SettingDtos()
    {
        _registerDto = new()
        {
            Username = "testuser",
            Email = "TestUser2@example.com",
            Password = "password123"
        };
        _authResponseDto = new()
        {
            RefreshToken = "MockRefreshToken",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            AccessToken = "MockAccessToken",
            Username = "testuser"
        };
        _loginDto = new UserLoginDto
        {
            Email = "TestUser2@example.com",
            Password = "TestUser2@example.com"
        };

    }

    [SetUp]
    public void Setup()
    {
        _mediator = Substitute.For<IMediator>();
        _authController = new AuthController(_mediator);
        SettingDtos();
        SettingHttpContext();
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
            Data = _authResponseDto
        };
        _mediator.Send(Arg.Is<RegisterCommand>(obj=>obj.RegisterDto.Equals(_registerDto)), Arg.Any<CancellationToken>())
            .Returns(response);
        //Act
        var result = await _authController.Register(_registerDto);
        var objectResult = result as ObjectResult;
        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        _mockResponseCookies.Received(1).Append("refreshToken", _authResponseDto.RefreshToken, Arg.Is<CookieOptions>(options =>
            options.HttpOnly == true &&
            options.Secure == true &&
            options.Expires == _authResponseDto.ExpiresAt
        ));
        Assert.That(objectResult?.Value, Is.EqualTo("Registered successfully"));
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
        _mediator.Send(Arg.Is<RegisterCommand>(obj=>obj.RegisterDto.Equals(_registerDto)), Arg.Any<CancellationToken>())
         .Returns(Task.FromResult(response));
        //Act
        var result = await _authController.Register(_registerDto);
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
            Data = _authResponseDto
        };

        _mediator.Send(Arg.Is<LoginCommand>(obj=>obj.LoginDto.Equals(_loginDto)), Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(response));

        //Act
        var result = await _authController.Login(_loginDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        Assert.That(objectResult?.Value, Is.EqualTo(_authResponseDto));

        _mockResponseCookies.Received(1).Append("refreshToken", _authResponseDto.RefreshToken, Arg.Is<CookieOptions>(options =>
            options.HttpOnly == true &&
            options.Secure == true &&
            options.Expires == _authResponseDto.ExpiresAt
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
        //
        _mediator.Send(Arg.Is<LoginCommand>(obj=> obj.LoginDto.Equals(_loginDto)), Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult(response));
        _mediator.Send(Arg.Is<LoginCommand>(obj=>obj.LoginDto.Email==_loginDto.Email && obj.LoginDto.Password==_loginDto.Password ), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(response));
        //Act
        var result = await _authController.Login(_loginDto);
        var objectResult = result as ObjectResult;

        //Assert
        Assert.That(objectResult?.StatusCode, Is.EqualTo(401));
        Assert.That(objectResult?.Value, Is.EqualTo(response.Messages));
    }
    [Test]
    public async Task Logout_WhenRefreshTokenExists_ReturnsOkAndDeletesRefreshTokens()
    {
        //Act
        var result = await _authController.Logout();
        var objectResult = result as ObjectResult;

        //Assert
        await _mediator.Received(1).Send(Arg.Is<LogoutCommand>(obj=>obj.RefreshToken is string), Arg.Any<CancellationToken>());
        _mockResponseCookies.Received(1).Delete("refreshToken");
        Assert.That(objectResult?.StatusCode, Is.EqualTo(200));
        Assert.That(objectResult?.Value, Is.EqualTo("Logged out successfully."));
    }
}
