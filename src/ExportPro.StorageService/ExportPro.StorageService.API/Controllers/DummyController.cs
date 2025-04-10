using ExportPro.Auth.SDK.DTOs;
using ExportPro.StorageService.SDK.Refit;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DummyController : ControllerBase
{
    private readonly IAuth _auth;

    public DummyController(IAuth authApi)
    {
        _auth = authApi;
    }
    [HttpPost("dummyregister")]
    [SwaggerOperation(Summary = "Register a new user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> DummyRegister([FromBody] UserRegisterDto dto)
    {
        var Register = await _auth.RegisterAsync(dto);
        return Ok(Register);
    }
    [HttpPost("dummylogin")]
    [SwaggerOperation(Summary = "Login a user")]
    [ProducesResponseType(typeof(AuthResponseDto), 200)]
    public async Task<IActionResult> DummyLogin([FromBody] UserLoginDto dto)
    {
        var response = await _auth.LoginAsync(dto);
        return Ok(response);
    }
}
