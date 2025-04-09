using ExportPro.Auth.CQRS.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ExportPro.Common.Shared.DTOs;
namespace ExportPro.Auth.ServiceHost.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var response = await _mediator.Send(new RegisterCommand(dto));
        if (!response.IsSuccess || response.Data == null)
            return BadRequest(response.Messages);

        SetRefreshTokenCookie(response.Data);
        return Ok("Registered successfully");
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login a user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var response = await _mediator.Send(new LoginCommand(dto));
        if (!response.IsSuccess || response.Data == null)
            return Unauthorized(response.Messages);

        SetRefreshTokenCookie(response.Data);
        return Ok(response.Data);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh token")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized("Refresh token is missing.");

        var response = await _mediator.Send(new RefreshTokenCommand(refreshToken));
        if (!response.IsSuccess || response.Data == null)
            return Unauthorized(response.Messages);

        SetRefreshTokenCookie(response.Data);
        return Ok(response.Data);
    }

    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Logout a user")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refreshToken", out var token))
        {
            await _mediator.Send(new LogoutCommand(token));
            Response.Cookies.Delete("refreshToken");
        }

        return Ok("Logged out successfully.");
    }

    private void SetRefreshTokenCookie(AuthResponseDto authResponse)
    {
        Response.Cookies.Append("refreshToken", authResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = authResponse.ExpiresAt
        });
    }
}
