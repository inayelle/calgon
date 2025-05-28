using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class DefaultGameFactory : IGameFactory
{
    private readonly IMapGenerator _defaultMapGenerator;
    private readonly Pipeline<GameContext> _gamePipeline;

    public DefaultGameFactory(IMapGenerator defaultMapGenerator, Pipeline<GameContext> gamePipeline)
    {
        _defaultMapGenerator = defaultMapGenerator;
        _gamePipeline = gamePipeline;
    }

    public Game CreateGame(Guid gameId, IGameEventDispatcher gameEventDispatcher)
    {
        var gameMap = _defaultMapGenerator.Generate();

        var gameContext = new GameContext(gameId, gameMap.Size, gameMap.Planets);

        return new Game(_gamePipeline, gameContext, gameEventDispatcher);
    }
}