using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookDepoSystem.Data.Models;

public class Feedback
{
    [Key]
    public Guid FeedbackId { get; set; }

    [Required]
    public Guid? BookId { get; set; }
    [ForeignKey("BookId")]
    public Book? Book { get; set; }

    [Required]
    public Guid? RenterId { get; set; }
    [ForeignKey("RenterId")]
    public Renter? Renter { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; }

    [Required]
    [Range(1, 5)]
    public float? Rate { get; set; }

    [Required]
    [MaxLength(1000)]
    public string? Message { get; set; }
}