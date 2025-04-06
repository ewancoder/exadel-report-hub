using System.Text.Json.Serialization;

namespace ExportPro.Common.Shared.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    [JsonIgnore]
    public string RefreshToken { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
