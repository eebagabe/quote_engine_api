using Microsoft.EntityFrameworkCore;
using QuoteEngine.Api.Models;

namespace QuoteEngine.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Traveler> Travelers => Set<Traveler>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quote>().Property(q => q.TotalFinal).HasPrecision(10, 2);
        modelBuilder.Entity<Traveler>().Property(t => t.Subtotal).HasPrecision(10, 2);
    }
}
