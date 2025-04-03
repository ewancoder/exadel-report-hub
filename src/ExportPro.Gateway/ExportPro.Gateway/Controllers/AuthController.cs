﻿using ExportPro.AuthService.Configuration;
using ExportPro.AuthService.Services;
using ExportPro.Common.DataAccess.MongoDB.Interfaces;
using ExportPro.Common.Shared.DTOs;
using ExportPro.Common.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.Gateway.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService, IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtOptions.Value;
    }

    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserRegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username);
        if (existingUser != null)
        {
            return Conflict("Username is already taken");
        }

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Roles = new List<UserRole> { UserRole.User }
        };

        user = await _userRepository.CreateAsync(user);

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Login to the system",
        Description = "Authenticates user credentials and returns a JWT token"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
        Summary = "Refresh JWT token",
        Description = "Generates a new JWT and refresh token if the provided refresh token is valid"
    )]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Unauthorized("Refresh token is missing.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            return Unauthorized("Invalid refresh token.");
        }

        var tokenObj = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);
        if (tokenObj == null || tokenObj.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Expired refresh token.");
        }

        return Ok(await GenerateTokenAndSetRefreshToken(user));
    }

    [HttpPost("logout")]
    [SwaggerOperation(
        Summary = "Logout user",
        Description = "Removes the refresh token from the user's tokens"
    )]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Logout()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            return Ok("User is already logged out.");
        }

        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user != null)
        {
            // Remove the refresh token from the user's list
            user.RefreshTokens.RemoveAll(rt => rt.Token == refreshToken);
            await _userRepository.UpdateAsync(user);
        }

        Response.Cookies.Delete("refreshToken");
        return Ok("Logged out successfully.");
    }

    // Private helper to clean expired tokens, generate new tokens and update the user
    private async Task<AuthResponseDto> GenerateTokenAndSetRefreshToken(User user)
    {
        // Clean up expired tokens
        user.RefreshTokens.RemoveAll(rt => rt.ExpiresAt <= DateTime.UtcNow);

        // Generate new JWT access token
        var authResponse = _jwtTokenService.GenerateToken(user);

        // Generate new refresh token
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            Token = newRefreshTokenValue,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
            CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
        };

        // Add new refresh token to the user's list and update the database
        user.RefreshTokens.Add(newRefreshToken);
        await _userRepository.UpdateAsync(user);

        // Set the new refresh token as an HttpOnly, Secure cookie
        Response.Cookies.Append("refreshToken", newRefreshTokenValue, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = newRefreshToken.ExpiresAt
        });

        return authResponse;
    }
}
