using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace BookDepoSystem.Data.Models;

public class Renter
{
    [Key]
    public Guid RenterId { get; set; }
    [Required]
    [MaxLength(30)]
    public string? Name { get; set; }
    [EmailAddress]
    [MaxLength(30)]
    public string? Email { get; set; }
    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}