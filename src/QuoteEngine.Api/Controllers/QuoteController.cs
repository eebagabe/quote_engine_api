using Microsoft.AspNetCore.Mvc;
using QuoteEngine.Api.Requests;
using QuoteEngine.Api.Services;

namespace QuoteEngine.Api.Controllers;

[ApiController]
[Route("api/quotes")]
public class QuoteController(QuoteService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Store(QuoteRequest request)
    {
        var result = await service.CalculateAndSaveAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var quotes = await service.ListAllAsync();
        return Ok(quotes);
    }
}
