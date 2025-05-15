using Calgon.Host.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Calgon.Host.Mvc;

internal sealed class CorsModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(ConfigureCors);
    }

    // TODO: add origins
    private static void ConfigureCors(CorsOptions corsOptions)
    {
        corsOptions.AddDefaultPolicy(builder =>
            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
        );
    }
}