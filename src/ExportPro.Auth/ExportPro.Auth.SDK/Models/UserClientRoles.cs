using ExportPro.Common.Models.MongoDB;
using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.Auth.SDK.Models;

public class UserClientRoles : IModel
{
    public ObjectId ClientId { get; set; }
    public ObjectId UserId { get; set; }
    public UserRole Role { get; set; }
    public ObjectId Id { get; set; }
}
