namespace QuoteEngine.Api.Services;

public record QuoteCalculation(
    int PricedDays,
    List<TravelerCalculation> Travelers,
    List<string> Warnings,
    int DiscountGroupPercentage,
    decimal TotalFinal);

public record TravelerCalculation(
    string Name,
    DateOnly BirthDate,
    int Age,
    decimal Subtotal,
    List<string> Addons,
    List<string> AddonsAllowed);
