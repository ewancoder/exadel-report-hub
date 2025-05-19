namespace ExportPro.Auth.SDK.DTOs;

public class UserInfo
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}
