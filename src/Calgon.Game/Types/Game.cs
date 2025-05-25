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

    public Game(Pipeline<GameContext> pipeline, GameContext context, IGameEventDispatcher eventDispatcher)
    {
        _pipeline = pipeline;
        _context = context;
        _eventDispatcher = eventDispatcher;

        _semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
    }

    public async Task Run()
    {
        await DispatchEvents(new GameStartedEvent());

        try
        {
            await Loop();
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async Task SendFleet(
        Guid departurePlanetId,
        Guid destinationPlanetId,
        float portion
    )
    {
        using var lease = await _semaphore.Acquire();

        if (!_context.Planets.TryGetValue(departurePlanetId, out var departurePlanet))
        {
            return;
        }

        if (!_context.Planets.TryGetValue(destinationPlanetId, out var destinationPlanet))
        {
            return;
        }

        if (!departurePlanet.TrySendFleet(destinationPlanet, portion, out var fleet))
        {
            return;
        }

        _context.Fleets.Add(fleet.Id, fleet);

        await DispatchEvents(new FleetSentEvent
            {
                Fleet = fleet,
            }
        );
    }

    private async Task Loop()
    {
        var timer = new PeriodicTimer(TickPeriod);

        while (await timer.WaitForNextTickAsync())
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

    private Task DispatchEvents(params IEnumerable<IGameEvent> events)
    {
        return _eventDispatcher.Dispatch(_context.Id, events);
    }
}