namespace QuoteEngine.Api.Models;

public class Traveler
{
    public long Id { get; set; }
    public long QuoteId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public int Age { get; set; }
    public decimal Subtotal { get; set; }
    public List<string> Addons { get; set; } = [];
    public List<string> AddonsAllowed { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
