using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ExportPro.Common.Shared.DTOs;

public class UserRegisterDto
{
    [Required]
    [Length(1, 50, ErrorMessage = "Username must be between {1} and {2} characters")]
    [SwaggerSchema(Description = "Username for registration")]
    public string Username { get; set; } = null!;

    [Required]
    [EmailAddress]
    [Length(1, 50, ErrorMessage = "Email Address must be between {1} and {2} characters")]
    [SwaggerSchema(Description = "Email Address for registration")]
    public string Email { get; set; } = null!;

    [Required]
    [Length(6, 100, ErrorMessage = "Password must be between {1} and {2} characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
    [SwaggerSchema(Description = "Enter the password")]
    public string Password { get; set; } = null!;
}
