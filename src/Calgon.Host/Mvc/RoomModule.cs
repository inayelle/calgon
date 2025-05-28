using Calgon.Host.Services;
using Calgon.Shared;

namespace Calgon.Host.Mvc;

public class RoomModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<RoomService>();
    }
}