using Microsoft.EntityFrameworkCore;
using QuoteEngine.Api.Data;
using QuoteEngine.Api.Models;
using QuoteEngine.Api.Requests;
using QuoteEngine.Api.Services;

namespace QuoteEngine.Api.Repositories;

public class QuoteRepository(AppDbContext db) : IQuoteRepository
{
    public async Task<Quote> SaveAsync(QuoteRequest request, QuoteCalculation result)
    {
        var now = DateTime.UtcNow;

        var quote = new Quote
        {
            Destination = request.Destination,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            PricedDays = result.PricedDays,
            DiscountGroupPercentage = result.DiscountGroupPercentage,
            TotalFinal = result.TotalFinal,
            Warnings = result.Warnings,
            CreatedAt = now,
            UpdatedAt = now,
            Travelers = result.Travelers.Select(t => new Traveler
            {
                Name = t.Name,
                BirthDate = t.BirthDate,
                Age = t.Age,
                Subtotal = t.Subtotal,
                Addons = t.Addons,
                AddonsAllowed = t.AddonsAllowed,
                CreatedAt = now,
                UpdatedAt = now,
            }).ToList(),
        };

        db.Quotes.Add(quote);
        await db.SaveChangesAsync();

        return quote;
    }

    public Task<List<Quote>> AllAsync()
    {
        return db.Quotes
            .Include(q => q.Travelers)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }
}
