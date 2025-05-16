using System.ComponentModel.DataAnnotations;

namespace ExportPro.Front.Models;
  public class RegisterModel
{
    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Username must be between {2} and {1} characters")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Email must be between {2} and {1} characters")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between {2} and {1} characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&.])[A-Za-z\d@$!%*?&.]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
    public string Password { get; set; } = string.Empty;
}