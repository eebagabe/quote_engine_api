namespace QuoteEngine.Api.Models;

public class Quote
{
    public long Id { get; set; }
    public string Destination { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int PricedDays { get; set; }
    public int DiscountGroupPercentage { get; set; }
    public decimal TotalFinal { get; set; }
    public List<string> Warnings { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<Traveler> Travelers { get; set; } = [];
}
