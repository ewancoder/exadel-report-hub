using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.SDK.DTOs;

public class ClientDto
{
    [Required]
    [Length(1, 50, ErrorMessage = "Name must be between {1} and {2} characters")]
    [SwaggerSchema(Description = "Name for client")]
    public required string Name { get; set; }

    [Required]
    [Length(1, 50, ErrorMessage = "Description must be between {1} and {2} characters")]
    [SwaggerSchema(Description = "Description for client")]
    public string? Description { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}
