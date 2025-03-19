using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookDepoSystem.Presentation.Models;

public class RentViewModel
{
    public Guid RentId { get; set; }

    [Required(ErrorMessage = "Rent Date is invalid.")]
    public DateTime RentDate { get; set; }
    // ?

    [Required(ErrorMessage = "Due Date is invalid.")]
    public DateTime DueDate { get; set; }
    // ?

    public DateTime ReturnDate { get; set; }
    // ?

    //[Required(ErrorMessage = "Status is required.")]
    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [Required(ErrorMessage = "Book is required.")]
    public Guid BookId { get; set; }

    public string? BookTitle { get; set; } // For display purposes

    [Required(ErrorMessage = "Renter is required.")]
    public Guid RenterId { get; set; }

    public string? RenterName { get; set; } // For display purposes

    public string? RentDateString { get; set; }
    public string? DueDateString { get; set; }
    public string? ReturnDateString { get; set; }
}