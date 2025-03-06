using System;
using System.ComponentModel.DataAnnotations;
using BookDepoSystem.Common;

namespace BookDepoSystem.Presentation.Models;

public class RenterViewModel
{
    public Guid RenterId { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "RenterNameIsRequiredErrorMessage")]
    [MaxLength(30)]
    public string? Name { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "RenterEmailIsRequiredErrorMessage")]
    [MaxLength(30)]
    public string? Email { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "RenterPhoneNumberIsRequiredErrorMessage")]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; } = string.Empty;
}