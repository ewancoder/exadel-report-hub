using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.StorageService.SDK.DTOs;

public class ClientDto
{
    public required string Name { get; set; }

    public string? Description { get; set; }
    public List<ItemDtoForClient>? Items { get; set; }
}
