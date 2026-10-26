using QuoteEngine.Api.Models;
using QuoteEngine.Api.Repositories;
using QuoteEngine.Api.Requests;

namespace QuoteEngine.Api.Services;

public class QuoteService(IQuoteRepository repository)
{
    public Task<Quote> CalculateAndSaveAsync(QuoteRequest request)
    {
        var result = Calculate(request);
        return repository.SaveAsync(request, result);
    }

    public Task<List<Quote>> ListAllAsync()
    {
        return repository.AllAsync();
    }

    public QuoteCalculation Calculate(QuoteRequest request)
    {
        decimal totalGroup = 0;

        var pricedDays = CalculatePricedDays(request.StartDate, request.EndDate);

        var travelers = new List<TravelerCalculation>();
        var warnings = new List<string>();

        foreach (var traveler in request.Travelers)
        {
            var age = CalculateAgeUntilTravelDate(traveler.BirthDate, request.StartDate);
            var addons = traveler.Addons ?? [];

            var basePrice = CalculateBasePrice(pricedDays, request.Destination, age);
            var (subtotal, addonsAllowed, addonWarnings) = SubtotalWithAddons(basePrice, addons, age, pricedDays, traveler.Name);

            warnings.AddRange(addonWarnings);

            travelers.Add(new TravelerCalculation(traveler.Name, traveler.BirthDate, age, subtotal, addons, addonsAllowed));
            totalGroup += subtotal;
        }

        var discountByGroup = DiscountByGroup(request.Travelers);

        var totalFinal = Math.Round(totalGroup - (totalGroup * discountByGroup / 100), 2, MidpointRounding.AwayFromZero);

        return new QuoteCalculation(pricedDays, travelers, warnings, discountByGroup, totalFinal);
    }

    private static int CalculatePricedDays(DateOnly startDate, DateOnly endDate)
    {
        var diffInDays = Math.Abs(endDate.DayNumber - startDate.DayNumber) + 1;
        return Math.Max(5, diffInDays);
    }

    private static int CalculateAgeUntilTravelDate(DateOnly birthDate, DateOnly startDate)
    {
        var age = startDate.Year - birthDate.Year;

        if (birthDate > startDate.AddYears(-age))
        {
            age--;
        }

        return age;
    }

    private static decimal CalculateBasePrice(int pricedDays, string destination, int age)
    {
        var ageMultiplier = age switch
        {
            <= 17 => 0.5m,
            <= 64 => 1.0m,
            _ => 2.0m,
        };

        var destinationMultiplier = destination switch
        {
            "NATIONAL" => 10,
            "AMERICAN" => 16,
            "EUROPE" => 22,
            _ => 0,
        };

        var basePrice = pricedDays * destinationMultiplier;

        return basePrice * ageMultiplier;
    }

    private static (decimal Subtotal, List<string> AddonsAllowed, List<string> Warnings) SubtotalWithAddons(
        decimal currentTravelerSubtotal, List<string> addons, int age, int pricedDays, string name)
    {
        var hasBaggage = false;
        var hasAdventureSports = false;
        var warnings = new List<string>();
        var addonsAllowed = new List<string>();

        foreach (var addon in addons)
        {
            if (addon == "ADVENTURE_SPORTS")
            {
                hasAdventureSports = true;
            }

            if (addon == "BAGGAGE")
            {
                hasBaggage = true;
                addonsAllowed.Add("BAGGAGE");
            }
        }

        if (hasAdventureSports && age >= 18 && age <= 64)
        {
            currentTravelerSubtotal += currentTravelerSubtotal * 0.25m;
            addonsAllowed.Add("ADVENTURE_SPORTS");
        }

        if (hasAdventureSports && (age <= 17 || age >= 65))
        {
            warnings.Add($"Esportes de Aventura não aplicado para {name}: fora da faixa etária permitida (18-64).");
        }

        if (hasBaggage)
        {
            currentTravelerSubtotal += 3 * pricedDays;
        }

        return (currentTravelerSubtotal, addonsAllowed, warnings);
    }

    private static int DiscountByGroup(List<TravelerRequest> travelers)
    {
        if (travelers.Count <= 4)
        {
            return 0;
        }

        return 10;
    }
}
