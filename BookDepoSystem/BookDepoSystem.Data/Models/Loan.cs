using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookDepoSystem.Data.Models;

public class Loan
{
    [Key]
    public int LoanID { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ReturnDate { get; set; }
    [Required]
    [MaxLength(20)]
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("BookID")]
    public Book? Book { get; set; }
    [ForeignKey("UserID")]
    public User? User { get; set; }
    [ForeignKey("AdminID")]
    public Admin? Admin { get; set; }
}