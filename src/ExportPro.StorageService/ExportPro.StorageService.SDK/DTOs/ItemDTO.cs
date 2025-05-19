using System.ComponentModel.DataAnnotations;

namespace ExportPro.StorageService.SDK.DTOs;

public sealed record ItemDTO
{
    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string? CustomerId { get; set; }

    [Required]
    public string? ClientId { get; set; }
}
