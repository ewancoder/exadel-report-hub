namespace ExportPro.Export.ServiceHost;

public class JwtSettings
{
    public required string Secret { get; set; }
    public int ExpirationInMinutes { get; set; }
    public int RefreshTokenExpirationInDays { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}