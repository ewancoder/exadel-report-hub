using ExportPro.Common.Models.MongoDB;
using MongoDB.Bson;

namespace ExportPro.Auth.SDK.Models;

public class UserClientRoles: IModel
{
   public ObjectId Id { get; set; }
   public ObjectId ClientId { get; set; }
   public ObjectId UserId { get; set; }
   public UserRole Role { get; set; } 
}

