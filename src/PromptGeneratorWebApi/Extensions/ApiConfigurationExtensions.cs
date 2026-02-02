using PromptGeneratorWebApi.Models;
using PromptGeneratorWebApi.Services;

namespace PromptGeneratorWebApi.Extensions;

/// <summary>
/// Extension methods for configuring the API middleware and endpoints.
/// </summary>
public static class ApiConfigurationExtensions
{
    /// <summary>
    /// Configures the API by setting up middleware pipeline and registering endpoints.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure.</param>
    /// <returns>The WebApplication instance for chaining.</returns>
    public static WebApplication ConfigureApi(this WebApplication app)
    {
        ConfigureMiddleware(app);
        MapEndpoints(app);
        return app;
    }

    /// <summary>
    /// Configures the HTTP request pipeline middleware.
    /// </summary>
    private static void ConfigureMiddleware(WebApplication app)
    {
        // Configure Swagger in development environment
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();
    }

    /// <summary>
    /// Maps all API endpoints.
    /// </summary>
    private static void MapEndpoints(WebApplication app)
    {
        MapPromptGenerationEndpoint(app);
        MapHealthCheckEndpoint(app);
    }

    /// <summary>
    /// Maps the POST /api/generate-prompt endpoint.
    /// </summary>
    private static void MapPromptGenerationEndpoint(WebApplication app)
    {
        app.MapPost("/api/generate-prompt", async (PromptRequest request, IPromptGeneratorService service) =>
        {
            if (string.IsNullOrWhiteSpace(request.Problem))
            {
                return Results.BadRequest(new PromptResponse
                {
                    Success = false,
                    Error = "Problem cannot be empty"
                });
            }

            try
            {
                var (analysis, context, optimizedPrompt) = await service.GenerateOptimizedPromptAsync(request.Problem);

                return Results.Ok(new PromptResponse
                {
                    Success = true,
                    OptimizedPrompt = optimizedPrompt,
                    Details = new PromptGenerationDetails
                    {
                        Analysis = analysis,
                        Context = context
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Error generating prompt"
                );
            }
        })
        .WithName("GeneratePrompt")
        .WithOpenApi()
        .WithDescription("Generates an optimized ChatGPT prompt from a user problem through a multi-stage AI pipeline");
    }

    /// <summary>
    /// Maps the GET /api/health endpoint.
    /// </summary>
    private static void MapHealthCheckEndpoint(WebApplication app)
    {
        app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithOpenApi()
            .WithDescription("Returns the health status of the API");
    }
}
