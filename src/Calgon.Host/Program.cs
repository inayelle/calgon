using Calgon.Game;
using Calgon.Host.Data;
using Calgon.Host.Game;
using Calgon.Host.Interfaces;
using Calgon.Host.Middlewares;
using Calgon.Host.Mvc;
using Calgon.Host.Services;
using Calgon.Shared;
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
    .AddModule<OpenApiModule>()
    .AddModule<RoomModule>()
    .AddModule<GameModule>()
    .AddModule<GameHubModule>()
    .AddModule<SignalRModule>();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseNpgsql(
        builder.Configuration["Infrastructure:ConnectionString"]
    )
);

builder.Services
    .AddScoped<CurrentUserMiddleware>()
    .AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCorsPolicy", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true) // Allow any origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("DynamicCorsPolicy");

app.MapControllers();
app.MapHub<GameHub>("/api/hubs/game");
app.MapHub<RoomHub>("/api/hubs/room");
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

app.UseMiddleware<CurrentUserMiddleware>();

using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}

await app.RunAsync();