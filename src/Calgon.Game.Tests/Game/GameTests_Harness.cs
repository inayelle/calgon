using System.Diagnostics;
using AnyKit.Pipelines;
using Calgon.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Calgon.Game.Tests.Game;

public partial class GameTests
{
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void SetUp()
    {
        var services = new ServiceCollection();

        services.AddModule<GameModule>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Trace.Flush();
    }

    private Harness CreateHarness()
    {
        var pipeline = _serviceProvider.GetRequiredService<Pipeline<GameContext>>();

        var context = new GameContext(
            gameId: Guid.NewGuid(),
            mapSize: 100,
            planets:
            [
                new Planet(new Location(10, 10), size: 10),
                new Planet(new Location(25, 25), size: 10),
                new Planet(new Location(50, 50), size: 10),
                new Planet(new Location(75, 75), size: 10),
                new Planet(new Location(100, 100), size: 10),
            ]
        );

        var dispatcher = SubstituteGameEventDispatcher.Create();

        var ticker = new ManualGameTicker(period: TimeSpan.FromMilliseconds(50));

        var game = new Calgon.Game.Game(ticker, pipeline, context, dispatcher);

        return new Harness
        {
            Game = game,
            Ticker = ticker,
            Context = context,
            Dispatcher = dispatcher,
        };
    }

    private sealed class Harness
    {
        public required Calgon.Game.Game Game { get; init; }
        public required ManualGameTicker Ticker { get; init; }

        public required GameContext Context { get; init; }
        public required IGameEventDispatcher Dispatcher { get; init; }

        public void Deconstruct(
            out Calgon.Game.Game game,
            out ManualGameTicker ticker,
            out GameContext context,
            out IGameEventDispatcher dispatcher
        )
        {
            game = Game;
            ticker = Ticker;
            context = Context;
            dispatcher = Dispatcher;
        }
    }
}