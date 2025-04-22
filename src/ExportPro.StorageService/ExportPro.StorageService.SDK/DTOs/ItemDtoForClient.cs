using System.ComponentModel.DataAnnotations;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public class ItemDtoForClient
{

    public required string Name { get; set; }
    public required string Description { get; set; }
    [Required]
    public double Price { get; set; }
    public Status Status { get; set; }
    public Currency Currency { get; set; } //maybe can be made into enum as well?

    
}
