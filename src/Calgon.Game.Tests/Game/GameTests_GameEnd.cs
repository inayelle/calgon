namespace Calgon.Game.Tests.Game;

public partial class GameTests
{
    [Test]
    public async Task Run_LastPlayer_Wins()
    {
        // arrange
        var (game, ticker, context, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        var player1 = context.Players.Values.ElementAt(0);
        var player2 = context.Players.Values.ElementAt(1);

        var departurePlanet = context.Planets.Values.First(planet => planet.Owner is null);
        departurePlanet.Accept(player1, ships: 10_000);

        var destinationPlanet = context.Planets.Values.First(planet => planet.Owner?.Id == player2.Id);

        var ticksToArrive = (int)MathF.Ceiling(
            departurePlanet.Location.DistanceTo(destinationPlanet.Location) / Fleet.Speed
        ) + 1;

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick();

        _ = Task.Run(() => game.SendFleet(player1.Id, departurePlanet.Id, destinationPlanet.Id, portion: 0.95f));

        await ticker.Tick(ticksToArrive);

        // assert
        Assert.That(game.State, Is.EqualTo(GameState.Ended));
        await dispatcher.Verify<PlayerEliminatedEvent>(game.Id);
        await dispatcher.Verify<GameEndedEvent>(game.Id);
    }
}