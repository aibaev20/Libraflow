using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BookDepoSystem.Data.Models;

public class Rent
{
    [Key]
    public Guid RentId { get; set; }

    public DateTime RentDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReturnDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string? Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [Required]
    public Guid? BookId { get; set; }
    [ForeignKey("BookId")]
    public Book? Book { get; set; }

    [Required]
    public Guid? RenterId { get; set; }
    [ForeignKey("RenterId")]
    public Renter? Renter { get; set; }

    public Guid? AdminId { get; set; } // The admin who created this book

    [ForeignKey("AdminId")]
    public ApplicationUser? CreatedByAdmin { get; set; }
}