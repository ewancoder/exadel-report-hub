using ExportPro.Common.Shared.Enums;
using MongoDB.Bson;

namespace ExportPro.StorageService.SDK.DTOs;
public class UserDto
{
    public ObjectId Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
}
