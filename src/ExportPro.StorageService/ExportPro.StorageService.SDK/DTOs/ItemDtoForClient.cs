using System.ComponentModel.DataAnnotations;
using ExportPro.StorageService.Models.Enums;
using ExportPro.StorageService.Models.Models;

namespace ExportPro.StorageService.SDK.DTOs;

public class ItemDtoForClient
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Description { get; set; }
    [Required]
    public double Price { get; set; }
    public Status Status { get; set; }
    public string CurrencyId { get; set; } //maybe can be made into enum as well?
}
