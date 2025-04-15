using System.ComponentModel.DataAnnotations;
using ExportPro.StorageService.Models.Enums;

namespace ExportPro.StorageService.SDK.DTOs;

public class ItemDtoForClient
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    public Status Status { get; set; }
    public Currency? Currency { get; set; } //maybe can be made into enum as well?
    
}
