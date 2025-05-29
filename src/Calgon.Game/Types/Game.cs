using AnyKit.Pipelines;
using Calgon.Shared;

namespace Calgon.Game;

public sealed class Game
{
    private static readonly TimeSpan TickPeriod = TimeSpan.FromMilliseconds(50);

    private readonly GameContext _context;
    private readonly Pipeline<GameContext> _pipeline;
    private readonly IGameEventDispatcher _eventDispatcher;

    private readonly SemaphoreSlim _semaphore;
    private readonly CancellationTokenSource _completion;

    public GameState State { get; private set; }

    public Guid Id => _context.Id;

    public Game(Pipeline<GameContext> pipeline, GameContext context, IGameEventDispatcher eventDispatcher)
    {
        _pipeline = pipeline;
        _context = context;
        _eventDispatcher = eventDispatcher;

        _semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        _completion = new CancellationTokenSource();

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

    }

    public async Task Run()
    {
        if (_context.Players.Count <= 1)
        {
            throw new InvalidOperationException("There should be at least two players in the game.");
        }

        await DispatchEvents(new GameStartedEvent
            {
                MapSize = _context.MapSize,
                TickPeriod = TickPeriod,
                FleetSpeed = Fleet.Speed,
                Planets = _context.Planets,
                Players = _context.Players,
            }
        );

        State = GameState.Running;

        try
        {
            await Loop();
        }
        catch (OperationCanceledException)
        {
        }
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

    private async Task Loop()
    {
        var timer = new PeriodicTimer(TickPeriod);

        while (await timer.WaitForNextTickAsync(_completion.Token))
        {
            using var lease = await _semaphore.Acquire();

            var events = Tick();

            await DispatchEvents(events);
        }
    }

    private IGameEvent[] Tick()
    {
        _pipeline.Invoke(_context);

        return _context.FlushEvents();
    }

    private Task DispatchEvents(params IReadOnlyCollection<IGameEvent> events)
    {
        if (events.OfType<GameEndedEvent>().Any())
        {
            _completion.Cancel();
            State = GameState.Ended;
        }

        return _eventDispatcher.Dispatch(_context.Id, events);
    }
}