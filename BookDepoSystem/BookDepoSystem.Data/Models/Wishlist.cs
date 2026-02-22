using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookDepoSystem.Data.Models;

public class Wishlist
{
    [Key]
    public Guid WishlistId { get; set; }

    [Required]
    public Guid? BookId { get; set; }
    [ForeignKey("BookId")]
    public Book? Book { get; set; }

    [Required]
    public Guid? RenterId { get; set; }
    [ForeignKey("RenterId")]
    public Renter? Renter { get; set; }

    public DateTime AddedOn { get; set; }
}