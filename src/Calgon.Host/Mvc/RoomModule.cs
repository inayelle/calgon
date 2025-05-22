using Calgon.Host.Extensions;
using Calgon.Host.Services;

namespace Calgon.Host.Mvc;

public class RoomModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<RoomService>();
    }
}