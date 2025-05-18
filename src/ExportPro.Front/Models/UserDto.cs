namespace ExportPro.Front.Models;
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<UserClientRolesDTO> ClientRoles { get; set; } = [];
    public Role Role { get; set; } = Role.None;
}
