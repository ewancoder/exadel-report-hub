using MongoDB.Bson;

namespace ExportPro.Auth.SDK.Models
{
    public class UserClientRoles
    {
        public ObjectId ClientId { get; set; }
        public required List<ObjectId> OwnerIds { get; set; } 
        public List<ObjectId>? ClientAdmins { get; set; }
        public List<ObjectId>? Operarors { get; set; }
    }
}
