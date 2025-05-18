namespace ExportPro.Front.Models;
public class UserClientRolesDTO
{
    public Guid ClientId { get; set; }
    public UserRole Role { get; set; }
    public Guid UserId { get; set; }
}
