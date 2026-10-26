using QuoteEngine.Api.Models;
using QuoteEngine.Api.Requests;
using QuoteEngine.Api.Services;

namespace QuoteEngine.Api.Repositories;

public interface IQuoteRepository
{
    Task<Quote> SaveAsync(QuoteRequest request, QuoteCalculation result);

    Task<List<Quote>> AllAsync();
}
