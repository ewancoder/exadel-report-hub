using ExportPro.Common.Models.MongoDB;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExportPro.Auth.SDK.Models;

public class User : IModel
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; } = Role.None;
    public List<RefreshToken> RefreshTokens { get; set; } = [];

    [BsonId]
    public ObjectId Id { get; set; }
}
