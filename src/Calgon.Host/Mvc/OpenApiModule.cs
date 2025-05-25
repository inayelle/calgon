using Calgon.Shared;

namespace Calgon.Host.Mvc;

internal sealed class OpenApiModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddOpenApi();
    }
}