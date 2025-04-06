using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Presentation.Models;

public class RegisterViewModel
{
    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsRequiredErrorMessage")]
    [EmailAddress(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "EmailIsInvalidErrorMessage")]
    public string? Email { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsRequiredErrorMessage")]
    public string? Password { get; set; }

    [Compare(
        nameof(Password),
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PasswordIsDifferentThanConfirmedErrorMessage")]
    public string? ConfirmPassword { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "UserNameIsRequiredErrorMessage")]
    public string? Name { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "UserPhoneNumberIsRequiredErrorMessage")]
    public string? PhoneNumber { get; set; }
}