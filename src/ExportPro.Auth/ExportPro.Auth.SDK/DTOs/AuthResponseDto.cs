using System.Text.Json.Serialization;

namespace ExportPro.Auth.SDK.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
