namespace ExportPro.Front.Models;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}

