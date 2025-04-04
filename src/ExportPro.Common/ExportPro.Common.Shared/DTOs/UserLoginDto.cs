using System.ComponentModel.DataAnnotations;

namespace ExportPro.Common.Shared.DTOs;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    [Length(1, 50, ErrorMessage = "Email Address must be between {1} and {2} characters")]
    public string Email { get; set; } = null!;

    [Required]
    [Length(6, 100, ErrorMessage = "Password must be between {1} and {2} characters")]
    public string Password { get; set; } = null!;
}
