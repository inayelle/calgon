using Calgon.Game;
using Calgon.Host.Services;
using Calgon.Shared;

namespace Calgon.Host.Game;

internal sealed class GameHubModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IGameEventDispatcher, GameHubEventDispatcher>()
            .AddSingleton<GameService>();
    }
}