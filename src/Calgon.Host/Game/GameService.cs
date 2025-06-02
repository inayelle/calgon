using System.Collections.Concurrent;
using Calgon.Game;

namespace Calgon.Host.Game;

public sealed class GameService
{
    private readonly IGameEventDispatcher _gameEventDispatcher;
    private readonly IGameFactory _gameFactory;
    private readonly ILogger<GameService> _logger;

    private readonly ConcurrentDictionary<Guid, Calgon.Game.Game> _games;

    public GameService(
        IGameEventDispatcher gameEventDispatcher,
        IGameFactory gameFactory,
        ILogger<GameService> logger
    )
    {
        _gameEventDispatcher = gameEventDispatcher;
        _gameFactory = gameFactory;
        _logger = logger;

        _games = new ConcurrentDictionary<Guid, Calgon.Game.Game>();
    }

    public void TryAddGame(Guid roomId)
    {
        _games.GetOrAdd(
            roomId,
            key => _gameFactory.CreateGame(key, _gameEventDispatcher)
        );
    }

    public void StartGame(Guid roomId)
    {
        var game = GetGame(roomId);

        if (game.State is not GameState.Idle)
        {
            throw new InvalidOperationException("Game is not idle.");
        }

        Task.Run(() => game.Run());

        _logger.LogInformation(
            "Game started. {GameId}",
            game.Id
        );
    }

    public async Task AddPlayer(Guid roomId, Guid playerId, string? playerName)
    {
        var game = GetGame(roomId);

        await game.AddPlayer(playerId, playerName);

        _logger.LogInformation(
            "Player added to the game started. {GameId} {PlayerId}",
            game.Id,
            playerId
        );
    }

    public async Task SendFleet(
        Guid roomId,
        Guid playerId,
        Guid departurePlanetId,
        Guid destinationPlanetId,
        float portion
    )
    {
        var game = GetGame(roomId);

        await game.SendFleet(
            playerId,
            departurePlanetId,
            destinationPlanetId,
            portion
        );
    }

    private Calgon.Game.Game GetGame(Guid roomId)
    {
        if (!_games.TryGetValue(roomId, out var game))
        {
            throw new ArgumentException("Game not found.", nameof(roomId));
        }

        return game;
    }
}