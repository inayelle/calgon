using AnyKit.Pipelines;
using Calgon.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Calgon.Game;

public sealed class GameModule : IServiceModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSingleton<IMapGenerator, DefaultMapGenerator>()
            .AddSingleton<IGameFactory, DefaultGameFactory>();

        services
            .AddSingleton(GamePipelineFactory);
    }

    private static Pipeline<GameContext> GamePipelineFactory(IServiceProvider provider)
    {
        var builder = new PipelineBuilder<GameContext>();

        builder.UsePipe(new AdvanceFleetsPipe().Invoke);
        builder.UsePipe(new LandFleetsPipe().Invoke);
        builder.UsePipe(new ProduceShipsPipe().Invoke);
        builder.UsePipe(new EliminatePlayersPipe().Invoke);
        builder.UsePipe(new EndGamePipe().Invoke);

        return builder.Build();
    }
}