using System.Globalization;
using System.Reflection.Metadata;
using BookDepoSystem.Common;
using BookDepoSystem.Data;
using BookDepoSystem.Data.Models;
using BookDepoSystem.Services.Contracts;
using BookDepoSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;

namespace BookDepoSystem.Services.Implementations;

public class RentService : IRentService
{
    private readonly EntityContext context;

    public RentService(EntityContext context)
    {
        this.context = context;
    }

    public async Task<Rent?> GetRentById(Guid rentId)
    {
        return await this.context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .FirstOrDefaultAsync(r => r.RentId == rentId);
    }

    public async Task AddRentAsync(Rent rent)
    {
        var book = await this.context.Books.FindAsync(rent.BookId);

        rent.RentId = Guid.NewGuid();
        rent.ReturnDate = DateTime.MinValue;
        rent.Status = @T.ConfirmedRent;
        rent.CreatedAt = DateTime.UtcNow;
        rent.UpdatedAt = DateTime.UtcNow;

        book!.QuantityAvailable -= 1;
        this.context.Books.Update(book);
        this.context.Rents.Add(rent);
        await this.context.SaveChangesAsync();
    }

    public async Task UpdateAllRentStatusesAsync()
    {
        var rents = await this.context.Rents.ToListAsync();

        foreach (var rent in rents)
        {
            if (rent.ReturnDate > rent.RentDate)
            {
                rent.Status = @T.CompletedRent;
            }
            else if (DateTime.Now >= rent.DueDate)
            {
                rent.Status = @T.OverdueRent;
            }
            else if (DateTime.Now < rent.RentDate)
            {
                rent.Status = @T.ConfirmedRent;
            }
            else
            {
                rent.Status = @T.ActiveRent;
            }

            rent.UpdatedAt = DateTime.UtcNow;
            this.context.Rents.Update(rent);
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<bool> UpdateReturnDateAsync(Guid rentId, DateTime returnDate, float? penalty)
    {
        var rent = await this.GetRentById(rentId);

        if (rent == null)
        {
            return false;
        }

        rent.ReturnDate = returnDate;
        rent.Status = @T.CompletedRent;
        rent.UpdatedAt = DateTime.UtcNow;
        rent.Penalty = penalty;

        if (rent.Book != null)
        {
            rent.Book.QuantityAvailable += 1;
        }

        this.context.Rents.Update(rent);

        await this.context.SaveChangesAsync();

        return true;
    }

    public async Task<(List<Rent> Rents, int TotalCount)> GetRentsPaginated(string status, string search, int pageIndex, int pageSize)
    {
        var query = this.context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .AsQueryable();

        var bookPopularity = this.context.Rents
            .GroupBy(r => r.BookId)
            .Select(g => new { BookId = g.Key, RentCount = g.Count() });

        query = query.Join(
                bookPopularity,
                rent => rent.BookId,
                pop => pop.BookId,
                (rent, pop) => new { Rent = rent, Popularity = pop.RentCount })
                .OrderByDescending(r => r.Popularity)
                .ThenByDescending(r => r.Rent.RentDate)
                .Select(r => r.Rent);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Status!.Contains(search) ||
                                     b.Book!.Title!.Contains(search) ||
                                     b.Renter!.Name!.Contains(search));
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(r => r.Status == status);
        }

        int totalCount = await query.CountAsync();
        var rents = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (rents, totalCount);
    }

    /// <summary>
    /// Repo -- https://github.com/QuestPDF/QuestPDF --link.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<RentPdfModel> ExportMonthlyRentsPdfAsync()
    {
        var document = CreateMonthlyRentsDocument();
        return await GenerateAndSavePdfAsync(await document, "monthly-rents");
    }

    public async Task<RentPdfModel> ExportYearlyReportPdfAsync(int year)
    {
        var document = CreateYearlyReportDocument(year);
        return await GenerateAndSavePdfAsync(await document, "yearly-report");
    }

    public async Task<RentPdfModel> ExportFinancialReportPdfAsync()
    {
        var document = CreateFinancialReportDocument();
        return await GenerateAndSavePdfAsync(await document, "financial-report");
    }

    private async Task<RentPdfModel> GenerateAndSavePdfAsync(Document document, string reportType)
    {
        byte[] pdfBytes = document.GeneratePdf();
        string fileName = $"{reportType}-{DateTime.Now:yyyy-MM-dd-HH-m-s}.pdf";
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await File.WriteAllBytesAsync(filePath, pdfBytes);

        return new RentPdfModel
        {
            FileName = fileName,
            File = pdfBytes,
        };
    }

    private async Task<Document> CreateMonthlyRentsDocument()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var rents = await this.context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .OrderBy(r => r.RentDate.Year)
            .ThenBy(r => r.RentDate.Month)
            .ThenBy(r => r.RentDate)
            .ToListAsync();

        var monthlyGroups = rents
            .GroupBy(r => new { r.RentDate.Year, r.RentDate.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.5f, Unit.Centimetre);
                page.Header()
                    .Text(@T.AdvancedMonthlyReportText)
                    .AlignCenter()
                    .Bold()
                    .FontSize(18);

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        foreach (var monthGroup in monthlyGroups)
                        {
                            column.Item()
                                .PaddingBottom(10)
                                .Text($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key.Month).ToUpper()} {monthGroup.Key.Year}")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentDateText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.DueDateText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.ReturnDateText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentStatusText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentBookText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentRenterText);
                                    });

                                    foreach (var rent in monthGroup)
                                    {
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.RentDate.ToString("dd-MM-yyyy HH:mm"));
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.DueDate.ToString("dd-MM-yyyy HH:mm"));
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.ReturnDate == DateTime.MinValue ? @T.UnreturnedBookText : rent.ReturnDate.ToString("dd-MM-yyyy HH:mm"));
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.Status);
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.Book?.Title);
                                        table.Cell().BorderBottom(1).Padding(5).Text(rent.Renter?.Name);
                                    }
                                });

                            column.Item().PaddingBottom(20);
                        }
                    });

                page.Footer()
                    .PaddingTop(25)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });
    }

    private async Task<Document> CreateYearlyReportDocument(int year)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var selectedYear = year;

        var rents = await context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .Where(r => r.RentDate.Year == selectedYear)
            .ToListAsync();

        var booksByMonth = rents
            .GroupBy(r => new { r.RentDate.Month, r.Book!.Title })
            .Select(g => new
            {
                g.Key.Month,
                Book = g.Key.Title,
                Count = g.Count(),
            })
            .OrderBy(x => x.Month)
            .ThenByDescending(x => x.Count)
            .ToList();

        var topRenters = rents
            .GroupBy(r => r.Renter!.Name)
            .Select(g => new
            {
                Renter = g.Key,
                Count = g.Count(),
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        var popularGenres = rents
            .GroupBy(r => r.Book!.Genre)
            .Select(g => new
            {
                Genre = g.Key,
                Count = g.Count(),
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.5f, Unit.Centimetre);
                page.Header()
                    .Text($"{@T.AdvancedYearlyReportText} – {year}")
                    .AlignCenter()
                    .Bold()
                    .FontSize(18);

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(6)
                            .Text(@T.TopRentedBooksByMonthText)
                            .FontSize(14)
                            .Bold();

                        foreach (var monthGroup in booksByMonth.GroupBy(x => x.Month))
                        {
                            column.Item()
                                .PaddingTop(5)
                                .Text(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key).ToUpper())
                                .FontSize(12)
                                .Bold();

                            column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentBookText);
                                        header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.NumberOfTopRentedBooks);
                                    });

                                    foreach (var item in monthGroup)
                                    {
                                        table.Cell().BorderBottom(1).Padding(5).Text(item.Book);
                                        table.Cell().BorderBottom(1).Padding(5).Text(item.Count.ToString());
                                    }
                                });
                        }

                        column.Item().PaddingTop(35);

                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(6)
                            .Text(@T.TopRentersText)
                            .FontSize(14)
                            .Bold();

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentRenterText);
                                    header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.NumberOfRentsByRenter);
                                });

                                foreach (var renter in topRenters)
                                {
                                    table.Cell().BorderBottom(1).Padding(5).Text(renter.Renter);
                                    table.Cell().BorderBottom(1).Padding(5).Text(renter.Count.ToString());
                                }
                            });

                        column.Item().PaddingTop(35);

                        column.Item()
                            .Background(Colors.Grey.Lighten3)
                            .Padding(6)
                            .Text(@T.PopularGenresText)
                            .FontSize(14)
                            .Bold();

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.BookGenreText);
                                    header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.NumberOfTopRentedBooks);
                                });

                                foreach (var genre in popularGenres)
                                {
                                    table.Cell().BorderBottom(1).Padding(5).Text(genre.Genre);
                                    table.Cell().BorderBottom(1).Padding(5).Text(genre.Count.ToString());
                                }
                            });
                    });

                page.Footer()
                    .PaddingTop(25)
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
        });
    }

    private async Task<Document> CreateFinancialReportDocument()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var rents = await this.context.Rents
            .Include(r => r.Book)
            .Include(r => r.Renter)
            .Where(r => r.Penalty > 0)
            .OrderByDescending(r => r.Penalty)
            .ThenBy(r => r.ReturnDate)
            .ToListAsync();

        var yearlyGroups = rents
            .GroupBy(r => r.RentDate.Year)
            .OrderBy(g => g.Key);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2.5f, Unit.Centimetre);
                page.Header()
                    .Text(@T.FinancialReportText)
                    .AlignCenter()
                    .Bold()
                    .FontSize(18);

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        foreach (var yearGroup in yearlyGroups)
                        {
                            var monthlyGroups = yearGroup
                                .GroupBy(r => r.RentDate.Month)
                                .OrderBy(g => g.Key);

                            foreach (var monthGroup in monthlyGroups)
                            {
                                column.Item()
                                    .PaddingBottom(10)
                                    .Text($"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthGroup.Key).ToUpper()} {yearGroup.Key}")
                                    .FontSize(14)
                                    .Bold();

                                column.Item()
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                            columns.RelativeColumn();
                                        });

                                        table.Header(header =>
                                        {
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentDateText);
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.DueDateText);
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.ReturnDateText);
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentBookText);
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentRenterText);
                                            header.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Indigo.Darken4)).Text(@T.RentPenaltyText);
                                        });

                                        foreach (var rent in monthGroup)
                                        {
                                            table.Cell().BorderBottom(1).Padding(5).Text(rent.RentDate.ToString("dd-MM-yyyy HH:mm"));
                                            table.Cell().BorderBottom(1).Padding(5).Text(rent.DueDate.ToString("dd-MM-yyyy HH:mm"));
                                            table.Cell().BorderBottom(1).Padding(5).Text(rent.ReturnDate == DateTime.MinValue ? @T.UnreturnedBookText : rent.ReturnDate.ToString("dd-MM-yyyy HH:mm"));
                                            table.Cell().BorderBottom(1).Padding(5).Text(rent.Book?.Title);
                                            table.Cell().BorderBottom(1).Padding(5).Text(rent.Renter?.Name);
                                            table.Cell().BorderBottom(1).Padding(5).DefaultTextStyle(x => x.Bold().FontColor(Colors.Red.Darken2)).Text($"{rent.Penalty?.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"))}");
                                        }
                                    });

                                column.Item().PaddingBottom(20);
                            }

                            var yearRents = yearGroup.ToList();
                            var yearRentCount = yearRents.Count;
                            var yearTotalPenalty = yearRents.Sum(r => r.Penalty);

                            column.Item()
                                .ShowOnce()
                                .PaddingBottom(30)
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text($"{@T.RentsPerYearWithPenaltiesText} {yearGroup.Key}: {yearRentCount}")
                                        .FontSize(13)
                                        .Bold()
                                        .FontColor(Colors.Indigo.Darken4);

                                    row.RelativeItem()
                                        .AlignRight()
                                        .Text($"{@T.TotalSumRentsWithPenaltiesText}: {yearTotalPenalty?.ToString("C", CultureInfo.CreateSpecificCulture("de-DE"))}")
                                        .FontSize(13)
                                        .Bold()
                                        .FontColor(Colors.Red.Darken3);
                                });
                        }
                    });

                page.Footer()
                    .PaddingTop(25)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });
    }
}