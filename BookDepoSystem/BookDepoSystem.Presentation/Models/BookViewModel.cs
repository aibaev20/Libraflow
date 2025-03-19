using System;
using System.ComponentModel.DataAnnotations;
using BookDepoSystem.Common;

namespace BookDepoSystem.Presentation.Models;

public class BookViewModel
{
    public Guid BookId { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "TitleIsRequiredErrorMessage")]
    [MaxLength(255)]
    public string? Title { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "AuthorIsRequiredErrorMessage")]
    [MaxLength(255)]
    public string? Author { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "GenreIsRequiredErrorMessage")]
    [MaxLength(100)]
    public string? Genre { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "InformationIsRequiredErrorMessage")]
    [MaxLength(1000)]
    public string? Information { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "LocationIsRequiredErrorMessage")]
    [MaxLength(1000)]
    public string? Location { get; set; } = string.Empty;

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "PublishedDateIsRequiredErrorMessage")]
    public DateTime? PublishedDate { get; set; }

    [Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "QuantityAvailableIsRequiredErrorMessage")]
    public int? QuantityAvailable { get; set; }
}