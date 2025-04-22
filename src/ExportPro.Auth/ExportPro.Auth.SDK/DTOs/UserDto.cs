using ExportPro.Common.Shared.Enums;
using ExportPro.Common.Shared.Models;

namespace ExportPro.Auth.SDK.DTOs;
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<UserClientRolesDTO> ClientRoles { get; set; } = [];
    public Role Role { get; set; } = Role.None;
}