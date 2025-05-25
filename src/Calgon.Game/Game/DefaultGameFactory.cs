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

    public Game CreateGame(IEnumerable<Player> players)
    {
        var gameMap = _defaultMapGenerator.Generate();

        var gameContext = new GameContext(players, gameMap.Planets);

        return new Game(_gamePipeline, gameContext);
    }
}