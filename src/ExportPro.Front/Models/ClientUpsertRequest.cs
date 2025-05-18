using System.ComponentModel.DataAnnotations;

namespace ExportPro.Front.Models;

public class ClientUpsertRequest
{
    public Guid Id { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 4, ErrorMessage = "Name must be between {2} and {1} characters")]
    public required string Name { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Username must be between {2} and {1} characters")]
    public required string Description { get; set; }
}

