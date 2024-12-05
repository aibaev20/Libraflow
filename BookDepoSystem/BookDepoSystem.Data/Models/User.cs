using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookDepoSystem.Data.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
    [EmailAddress]
    public string? Email { get; set; }

    [ForeignKey("AdminID")]
    public Admin? CreatedByAdmin { get; set; }
}