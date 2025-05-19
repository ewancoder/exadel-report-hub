using ExportPro.Common.Shared.Enums;

namespace ExportPro.Auth.SDK.DTOs;

public record CreateUpdateUserDTO
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole? Role { get; set; }
    public Guid? ClientId { get; set; }
}
