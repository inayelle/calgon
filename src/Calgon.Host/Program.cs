using Calgon.Host.Data;
using Calgon.Host.Extensions;
using Calgon.Host.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration
    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
    .AddJsonFile("infrastructure.json", optional: false, reloadOnChange: true)
    .AddJsonFile("application.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables("CALGON_");

builder.Services.AddAuthorization();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;

    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
});

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder
    .Services
    .AddModule<MvcModule>()
    .AddModule<OpenApiModule>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        builder.Configuration["Infrastructure:ConnectionString"]
    )
);

var app = builder.Build();

app.MapControllers();
app.MapIdentityApi<IdentityUser>();
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

using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

await app.RunAsync();