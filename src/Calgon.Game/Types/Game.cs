using System.Diagnostics;
using AnyKit.Pipelines;
using Calgon.Shared;

namespace Calgon.Game;

public sealed class Game
{
    private readonly IGameTicker _ticker;
    private readonly GameContext _context;
    private readonly Pipeline<GameContext> _pipeline;
    private readonly IGameEventDispatcher _eventDispatcher;

    private readonly List<Player> _players;
    private readonly SemaphoreSlim _semaphore;

    public GameState State { get; private set; }

    public Guid Id => _context.Id;

    public IReadOnlyList<Player> Players => _players;

    public Game(
        IGameTicker ticker,
        Pipeline<GameContext> pipeline,
        GameContext context,
        IGameEventDispatcher eventDispatcher
    )
    {
        _ticker = ticker;
        _pipeline = pipeline;
        _context = context;
        _eventDispatcher = eventDispatcher;

        _players = new List<Player>(capacity: 6);
        _semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        State = GameState.Idle;
    }

    public async Task AddPlayer(Guid playerId, string? playerName)
    {
        using var lease = await _semaphore.Acquire();

        if (State is not GameState.Idle)
        {
            throw new InvalidOperationException("Players should be added while in Idle state.");
        }

        var player = new Player(
            playerId,
            playerName,
            color: _context.Players.Count
        );

        if (!_context.TryAddPlayer(player))
        {
            throw new InvalidOperationException("Couldn't add a player to the game.");
        }

        _players.Add(player);
    }

    public async Task<Player> Run()
    {
        if (_context.Players.Count <= 1)
        {
            throw new InvalidOperationException("There should be at least two players in the game.");
        }

        await DispatchEvents(new GameStartedEvent
            {
                MapSize = _context.MapSize,
                TickPeriod = _ticker.Period,
                FleetSpeed = Fleet.Speed,
                Planets = _context.Planets,
                Players = _context.Players,
            }
        );

        State = GameState.Running;

        return await Loop();
    }

    public async Task SendFleet(
        Guid playerId,
        Guid departurePlanetId,
        Guid destinationPlanetId,
        float portion
    )
    {
        using var lease = await _semaphore.Acquire();

        if (!_context.Players.TryGetValue(playerId, out var player))
        {
            return;
        }

        if (!_context.Planets.TryGetValue(departurePlanetId, out var departurePlanet))
        {
            return;
        }

        if (!_context.Planets.TryGetValue(destinationPlanetId, out var destinationPlanet))
        {
            return;
        }

        if (!player.Equals(departurePlanet.Owner))
        {
            return;
        }

        if (!departurePlanet.TrySendFleet(destinationPlanet, portion, out var fleet))
        {
            return;
        }

        _context.AddFleet(fleet);

        await DispatchEvents(new FleetSentEvent
            {
                Fleet = fleet,
            }
        );
    }

    private async Task<Player> Loop()
    {
        while (await _ticker.WaitForNextTickAsync())
        {
            using var lease = await _semaphore.Acquire();

            var events = Tick();

            await DispatchEvents(events);

            var gameEndedEvent = events.OfType<GameEndedEvent>().FirstOrDefault();

            if (gameEndedEvent is null)
            {
                continue;
            }

            State = GameState.Ended;
            return gameEndedEvent.Winner;
        }

        throw new UnreachableException("The game always ends within the game loop.");
    }

    private IGameEvent[] Tick()
    {
        _pipeline.Invoke(_context);

        return _context.FlushEvents();
    }

    private Task DispatchEvents(params IReadOnlyCollection<IGameEvent> events)
    {
        return _eventDispatcher.Dispatch(_context.Id, events);
    }
}