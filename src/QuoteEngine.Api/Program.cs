using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuoteEngine.Api.Data;
using QuoteEngine.Api.Repositories;
using QuoteEngine.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower)
    .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context =>
        new UnprocessableEntityObjectResult(new ValidationProblemDetails(context.ModelState)));

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<QuoteService>();

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()));

var app = builder.Build();

app.UseCors();

app.MapControllers();

app.Run();
