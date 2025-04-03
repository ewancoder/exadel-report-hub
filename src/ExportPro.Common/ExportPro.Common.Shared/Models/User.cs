using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ExportPro.Common.Shared.Models;

// TODO: Implement IModel, probably in 'ExportPro.Common.Shared'
// public class User : IModel
public class User
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public int TokenVersion { get; set; } = 0;
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
