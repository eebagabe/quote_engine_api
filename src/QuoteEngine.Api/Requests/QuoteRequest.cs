using System.ComponentModel.DataAnnotations;

namespace QuoteEngine.Api.Requests;

public class QuoteRequest : IValidatableObject
{
    [Required, AllowedValues("NATIONAL", "AMERICAN", "EUROPE")]
    public required string Destination { get; init; }

    public required DateOnly StartDate { get; init; }

    public required DateOnly EndDate { get; init; }

    [MinLength(1)]
    public required List<TravelerRequest> Travelers { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("The end date must be a date after or equal to start date.", [nameof(EndDate)]);
        }
    }
}

public class TravelerRequest
{
    [Required]
    public required string Name { get; init; }

    public required DateOnly BirthDate { get; init; }

    public List<string>? Addons { get; init; }
}
