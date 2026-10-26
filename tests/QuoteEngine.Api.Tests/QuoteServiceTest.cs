using QuoteEngine.Api.Requests;
using QuoteEngine.Api.Services;

namespace QuoteEngine.Api.Tests;

public class QuoteServiceTest
{
    // Calculate never touches the repository.
    private readonly QuoteService _service = new(null!);

    private static QuoteRequest Request(string destination, string startDate, string endDate, params TravelerRequest[] travelers) => new()
    {
        Destination = destination,
        StartDate = DateOnly.Parse(startDate),
        EndDate = DateOnly.Parse(endDate),
        Travelers = [.. travelers],
    };

    private static TravelerRequest Traveler(string name, string birthDate, params string[] addons) => new()
    {
        Name = name,
        BirthDate = DateOnly.Parse(birthDate),
        Addons = [.. addons],
    };

    [Fact]
    public void ShortTripRequiresAtLeast5Days()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-11", Traveler("Leo", "1990-01-01")));

        Assert.Equal(5, result.PricedDays);
    }

    [Fact]
    public void AgeIsCalculatedBasedOnStartDate()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19", Traveler("Gabriel", "2008-07-15")));

        Assert.Equal(17, result.Travelers[0].Age);
    }

    [Fact]
    public void AgeMultiplierForChild()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19", Traveler("Luan Candido", "2016-06-10")));

        Assert.Equal(50.0m, result.Travelers[0].Subtotal);
    }

    [Fact]
    public void AgeMultiplierForAdult()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19", Traveler("Gabriel Alves", "2001-07-27")));

        Assert.Equal(100.0m, result.Travelers[0].Subtotal);
    }

    [Fact]
    public void AgeMultiplierForSenior()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19", Traveler("Roger Flores", "1950-06-10")));

        Assert.Equal(200.0m, result.Travelers[0].Subtotal);
    }

    [Fact]
    public void AdventureSportsDeniedWithWarning()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19",
            Traveler("Roger Flores", "1950-06-10", "ADVENTURE_SPORTS")));

        Assert.Equal(
            "Esportes de Aventura não aplicado para Roger Flores: fora da faixa etária permitida (18-64).",
            result.Warnings[0]);

        Assert.Equal(200.0m, result.Travelers[0].Subtotal);
    }

    [Fact]
    public void DiscountGroupPercentageFewerThan5Travelers()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19",
            Traveler("Juliano", "1999-06-10", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Gabriel", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE")));

        Assert.Equal(0, result.DiscountGroupPercentage);
    }

    [Fact]
    public void DiscountGroupPercentageMoreThan5Travelers()
    {
        var result = _service.Calculate(Request("NATIONAL", "2026-07-10", "2026-07-19",
            Traveler("Juliano", "1999-06-10", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Gabriel", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Lucas", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Vinicius", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Pedro", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE"),
            Traveler("Joyce", "2001-07-27", "ADVENTURE_SPORTS", "BAGGAGE")));

        Assert.Equal(10, result.DiscountGroupPercentage);
    }

    [Fact]
    public void CompleteScenarioWithMultipleTravelersAndAddons()
    {
        var result = _service.Calculate(Request("EUROPE", "2026-07-10", "2026-07-20",
            Traveler("Ana", "1990-03-15", "BAGGAGE", "ADVENTURE_SPORTS"),
            Traveler("Joao", "1948-11-02", "ADVENTURE_SPORTS", "BAGGAGE")));

        Assert.Equal(11, result.PricedDays);

        Assert.Equal(335.5m, result.Travelers[0].Subtotal);

        Assert.Equal(517.0m, result.Travelers[1].Subtotal);

        Assert.Equal(852.5m, result.TotalFinal);

        Assert.Equal(
            "Esportes de Aventura não aplicado para Joao: fora da faixa etária permitida (18-64).",
            result.Warnings[0]);
    }
}
