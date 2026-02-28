using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Presentation.Models;

public class FeedbackViewModel
{
    public Guid BookId { get; set; }
    public string? BookTitle { get; set; }

    public Guid RenterId { get; set; }
    public string? RenterName { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "RateIsRequiredErrorMessage")]
    [Range(1, 5)]
    public float Rate { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "MessageIsRequiredErrorMessage")]
    [MaxLength(1000)]
    public string? Message { get; set; }
}