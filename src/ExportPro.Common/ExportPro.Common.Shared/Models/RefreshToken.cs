namespace ExportPro.Common.Shared.Models;

public class RefreshToken
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByIp { get; set; } = null!;
}
