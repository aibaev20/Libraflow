using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookDepoSystem.Presentation.Models;

public class RentViewModel
{
    public Guid RentId { get; set; }

    /*[Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "RentDateIsInvalidErrorMessage")]*/
    public DateTime RentDate { get; set; }
    // ?

    /*[Required(
        ErrorMessageResourceType = typeof(Common.T),
        ErrorMessageResourceName = "DueDateIsInvalidErrorMessage")]*/
    public DateTime DueDate { get; set; }
    // ?

    public DateTime ReturnDate { get; set; }
    // ?

    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Guid BookId { get; set; }

    public string? BookTitle { get; set; }

    public Guid RenterId { get; set; }

    public string? RenterName { get; set; }

    public string? ReturnDateString { get; set; }
}