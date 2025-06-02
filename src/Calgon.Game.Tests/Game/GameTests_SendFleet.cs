namespace Calgon.Game.Tests.Game;

public partial class GameTests
{
    private const float FleetPortion = 0.5f;

    [Test]
    public async Task SendFleet_FromOwnPlanetToEmptyPlanet_CapturesPlanet()
    {
        // arrange
        var (game, ticker, context, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        var player1 = context.Players.Values.ElementAt(0);

        var departurePlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner?.Id == player1.Id);

        var destinationPlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner is null);

        Assume.That(departurePlanet, Is.Not.Null);
        Assume.That(destinationPlanet, Is.Not.Null);

        var minExpectedShipsInFleet = (int)MathF.Floor(departurePlanet.Ships * FleetPortion);

        Assume.That(minExpectedShipsInFleet, Is.GreaterThan(0));

        var ticksToArrive = (int)MathF.Ceiling(
            departurePlanet.Location.DistanceTo(destinationPlanet.Location) / Fleet.Speed
        );

        FleetSentEvent? fleetSentEvent = null;
        FleetArrivedEvent? fleetArrivedEvent = null;

        dispatcher
            .When<FleetSentEvent>(game.Id)
            .Then<FleetSentEvent>(events => fleetSentEvent = events.First());

        dispatcher
            .When<FleetArrivedEvent>(game.Id)
            .Then<FleetArrivedEvent>(events => fleetArrivedEvent = events.First());

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick();

        _ = Task.Run(() => game.SendFleet(player1.Id, departurePlanet.Id, destinationPlanet.Id, FleetPortion));

        await ticker.Tick();

        // assert
        Assert.That(fleetSentEvent, Is.Not.Null);
        Assert.That(fleetSentEvent.Fleet, Is.Not.Null);
        Assert.That(fleetSentEvent.Fleet.DeparturePlanet.Id, Is.EqualTo(departurePlanet.Id));
        Assert.That(fleetSentEvent.Fleet.DestinationPlanet.Id, Is.EqualTo(destinationPlanet.Id));
        Assert.That(fleetSentEvent.Fleet.Owner.Id, Is.EqualTo(player1.Id));
        Assert.That(fleetSentEvent.Fleet.Ships, Is.GreaterThanOrEqualTo(minExpectedShipsInFleet));

        // wait for the arrival
        await ticker.Tick(ticksToArrive);

        // assert
        Assert.That(fleetArrivedEvent, Is.Not.Null);
        Assert.That(fleetArrivedEvent.Fleet.DestinationPlanet.Owner, Is.Not.Null);
        Assert.That(fleetArrivedEvent.Fleet.DestinationPlanet.Owner.Id, Is.EqualTo(player1.Id));
        Assert.That(fleetArrivedEvent.Fleet.DestinationPlanet.Ships, Is.GreaterThanOrEqualTo(minExpectedShipsInFleet));
    }

    [Test]
    public async Task SendFleet_FromOwnPlanetToAnotherOwnPlanet_ReinforcesPlanet()
    {
        // arrange
        var (game, ticker, context, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        var player1 = context.Players.Values.ElementAt(0);

        var departurePlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner?.Id == player1.Id);

        var destinationPlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner is null);

        Assume.That(departurePlanet, Is.Not.Null);
        Assume.That(destinationPlanet, Is.Not.Null);

        destinationPlanet.Accept(player1, ships: 10);

        var minExpectedShipsInFleet = (int)MathF.Floor(departurePlanet.Ships * FleetPortion);
        var minExpectedShipsOnDestinationPlanet = destinationPlanet.Ships + minExpectedShipsInFleet;

        Assume.That(minExpectedShipsInFleet, Is.GreaterThan(0));

        var ticksToArrive = (int)MathF.Ceiling(
            departurePlanet.Location.DistanceTo(destinationPlanet.Location) / Fleet.Speed
        );

        FleetSentEvent? fleetSentEvent = null;
        FleetArrivedEvent? fleetArrivedEvent = null;

        dispatcher
            .When<FleetSentEvent>(game.Id)
            .Then<FleetSentEvent>(events => fleetSentEvent = events.Single());

        dispatcher
            .When<FleetArrivedEvent>(game.Id)
            .Then<FleetArrivedEvent>(events => fleetArrivedEvent = events.Single());

        // act
        _ = Task.Run(() => game.Run());

        // wait for the first tick
        await ticker.Tick();

        _ = Task.Run(() => game.SendFleet(player1.Id, departurePlanet.Id, destinationPlanet.Id, FleetPortion));

        // wait for the tick
        await ticker.Tick();

        // assert
        Assert.That(fleetSentEvent, Is.Not.Null);

        // wait for the arrival
        await ticker.Tick(ticksToArrive);

        // assert
        Assert.That(fleetArrivedEvent, Is.Not.Null);
        Assert.That(
            fleetArrivedEvent.Fleet.DestinationPlanet.Ships,
            Is.GreaterThanOrEqualTo(minExpectedShipsOnDestinationPlanet)
        );
    }

    [Test]
    public async Task SendFleet_FromOwnPlanetToEnemyWeakerPlanet_WinsBattle()
    {
        // arrange
        var (game, ticker, context, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        var player1 = context.Players.Values.ElementAt(0);
        var player2 = context.Players.Values.ElementAt(1);

        var departurePlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner?.Id == player1.Id);

        var destinationPlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner is null);

        Assume.That(departurePlanet, Is.Not.Null);
        Assume.That(destinationPlanet, Is.Not.Null);

        destinationPlanet.Accept(player2, ships: 1);

        var minExpectedShipsInFleet = (int)MathF.Floor(departurePlanet.Ships * FleetPortion);

        Assume.That(minExpectedShipsInFleet, Is.GreaterThan(0));

        var ticksToArrive = (int)MathF.Ceiling(
            departurePlanet.Location.DistanceTo(destinationPlanet.Location) / Fleet.Speed
        );

        FleetSentEvent? fleetSentEvent = null;
        FleetArrivedEvent? fleetArrivedEvent = null;

        dispatcher
            .When<FleetSentEvent>(game.Id)
            .Then<FleetSentEvent>(events => fleetSentEvent = events.Single());

        dispatcher
            .When<FleetArrivedEvent>(game.Id)
            .Then<FleetArrivedEvent>(events => fleetArrivedEvent = events.Single());

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick();

        _ = Task.Run(() => game.SendFleet(player1.Id, departurePlanet.Id, destinationPlanet.Id, FleetPortion));

        await ticker.Tick();

        // assert
        Assert.That(fleetSentEvent, Is.Not.Null);

        await ticker.Tick(ticksToArrive);

        // assert
        Assert.That(fleetArrivedEvent, Is.Not.Null);
        Assert.That(fleetArrivedEvent.Fleet.DestinationPlanet.Owner, Is.Not.Null);
        Assert.That(
            fleetArrivedEvent.Fleet.DestinationPlanet.Owner.Id,
            Is.EqualTo(player1.Id)
        );
    }

    [Test]
    public async Task SendFleet_FromOwnPlanetToEnemyStrongerPlanet_LosesBattle()
    {
        // arrange
        var (game, ticker, context, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        var player1 = context.Players.Values.ElementAt(0);
        var player2 = context.Players.Values.ElementAt(1);

        var departurePlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner?.Id == player1.Id);

        var destinationPlanet = context
            .Planets
            .Values
            .FirstOrDefault(planet => planet.Owner is null);

        Assume.That(departurePlanet, Is.Not.Null);
        Assume.That(destinationPlanet, Is.Not.Null);

        destinationPlanet.Accept(player2, ships: 1000);

        var minExpectedShipsInFleet = (int)MathF.Floor(departurePlanet.Ships * FleetPortion);

        Assume.That(minExpectedShipsInFleet, Is.GreaterThan(0));

        var ticksToArrive = (int)MathF.Ceiling(
            departurePlanet.Location.DistanceTo(destinationPlanet.Location) / Fleet.Speed
        );

        FleetSentEvent? fleetSentEvent = null;
        FleetArrivedEvent? fleetArrivedEvent = null;

        dispatcher
            .When<FleetSentEvent>(game.Id)
            .Then<FleetSentEvent>(events => fleetSentEvent = events.Single());

        dispatcher
            .When<FleetArrivedEvent>(game.Id)
            .Then<FleetArrivedEvent>(events => fleetArrivedEvent = events.Single());

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick();

        _ = Task.Run(() => game.SendFleet(player1.Id, departurePlanet.Id, destinationPlanet.Id, FleetPortion));

        await ticker.Tick();

        // assert
        Assert.That(fleetSentEvent, Is.Not.Null);

        await ticker.Tick(ticksToArrive);

        // assert
        Assert.That(fleetArrivedEvent, Is.Not.Null);
        Assert.That(fleetArrivedEvent.Fleet.DestinationPlanet.Owner, Is.Not.Null);
        Assert.That(
            fleetArrivedEvent.Fleet.DestinationPlanet.Owner.Id,
            Is.EqualTo(player2.Id)
        );
    }
}