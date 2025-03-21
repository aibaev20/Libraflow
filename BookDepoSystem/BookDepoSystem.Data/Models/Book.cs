using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookDepoSystem.Data.Models;

public class Book
{
    [Key]
    public Guid BookId { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Author { get; set; }
    [MaxLength(50)]
    public string? Genre { get; set; }
    public string? Information { get; set; }
    [MaxLength(20)]
    public string? Location { get; set; }
    public DateTime PublishedDate { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityAvailable { get; set; }
    public string? CoverImage { get; set; }
    public Guid? AdminId { get; set; } // The admin who created this book

    [ForeignKey("AdminId")]
    public ApplicationUser? CreatedByAdmin { get; set; }
}