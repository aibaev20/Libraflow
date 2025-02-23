namespace BookDepoSystem.Presentation.Models;

public class PaginationViewModel<T>
{
    public List<T> Items { get; set; } = new();
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }

    public bool HasPreviousPage => this.PageIndex > 1;
    public bool HasNextPage => this.PageIndex < this.TotalPages;
}