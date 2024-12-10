using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Presentation.Models;

public class ForgotPasswordViewModel
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    [EmailAddress(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
    public string? Email { get; set; }
}