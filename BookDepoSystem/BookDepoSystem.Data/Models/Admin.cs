using System.ComponentModel.DataAnnotations;

namespace BookDepoSystem.Data.Models;

public class Admin
{
    [Key]
    public int AdminID { get; set; }
    [Required]
    [MaxLength(50)]
    public string? Username { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? PasswordHash { get; set; }
}