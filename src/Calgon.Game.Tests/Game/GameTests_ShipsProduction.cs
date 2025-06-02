namespace Calgon.Game.Tests.Game;

public partial class GameTests
{
    [Test]
    public async Task Run_Periodically_PlayerPlanetsProduceShips()
    {
        // arrange
        var (game, ticker, _, dispatcher) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick(ticks: 75);

        // assert
        await dispatcher.Verify<ShipsProducedEvent>(game.Id);
    }
}