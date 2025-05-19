namespace ExportPro.Front.Models;

public class UserDtoInt
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public int? Role { get; set; }
    public Guid? ClientId { get; set; }
}
