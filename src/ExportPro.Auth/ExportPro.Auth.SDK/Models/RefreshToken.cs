using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Auth.SDK.Models;

[BsonIgnoreExtraElements]
public class RefreshToken
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TokenVersion { get; set; } = 0;
}
