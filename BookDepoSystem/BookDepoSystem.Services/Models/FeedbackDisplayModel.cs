namespace BookDepoSystem.Services.Models;

public class FeedbackDisplayModel
{
    public string? RenterName { get; set; }
    public float? Rate { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedOn { get; set; }
}