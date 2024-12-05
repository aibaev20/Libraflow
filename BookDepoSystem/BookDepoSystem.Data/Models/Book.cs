using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Data.Models;

public class Book
{
    [Key]
    public int BookID { get; set; }
    [Required]
    [MaxLength(255)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Author { get; set; }
    [MaxLength(50)]
    public string? Genre { get; set; }
    public string? Information { get; set; }
    public DateTime? PublishedDate { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public int QuantityAvailable { get; set; }
}