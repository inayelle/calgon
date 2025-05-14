using Calgon.Host.Extensions;
using Calgon.Host.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
    .AddJsonFile("infrastructure.json", optional: false, reloadOnChange: true)
    .AddJsonFile("application.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables("CALGON_");

builder
    .Services
    .AddModule<MvcModule>()
    .AddModule<OpenApiModule>();

var app = builder.Build();

app.MapControllers();
app.MapOpenApi("/_/openapi/v1.json");
app.MapScalarApiReference(
    "/_/openapi/scalar",
    scalar =>
    {
        scalar
            .WithOpenApiRoutePattern("/_/openapi/v1.json")
            .WithDarkMode(true)
            .WithTheme(ScalarTheme.DeepSpace);
    }
);

await app.RunAsync();