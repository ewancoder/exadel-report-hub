
using MongoDB.Bson;

namespace ExportPro.Common.Models.MongoDB.Models;

public class Customer : IModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}

