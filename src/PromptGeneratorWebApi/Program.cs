using PromptGeneratorWebApi.Extensions;
using PromptGeneratorWebApi.Models;
using PromptGeneratorWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register HttpClient and PromptGeneratorService
builder.Services.AddHttpClient<IPromptGeneratorService, PromptGeneratorService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the API (middleware and endpoints)
app.ConfigureApi();

app.Run();

// Make Program public for testing
public partial class Program { }
