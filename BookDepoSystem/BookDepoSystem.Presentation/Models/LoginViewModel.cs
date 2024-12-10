using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Presentation.Models;

public class LoginViewModel
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    public string? Email { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}