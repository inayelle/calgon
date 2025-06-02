namespace Calgon.Game.Tests.Game;

public partial class GameTests
{
    [Test]
    public async Task Run_WithTwoPlayersAdded_StartsGame()
    {
        // arrange
        var (game, ticker, _, _) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        // act
        _ = Task.Run(() => game.Run());

        // 0th tick
        await ticker.Tick();

        // assert
        Assert.That(game.State, Is.EqualTo(GameState.Running));
    }

    [Test]
    public async Task Run_WithLessThanTwoPlayersAdded_ThrowsInvalidOperationException()
    {
        // arrange
        var (game, _, _, _) = CreateHarness();

        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");

        // act & assert
        Assert.ThrowsAsync<InvalidOperationException>(() => game.Run());
    }

    [Test]
    public async Task Run_WithTwoPlayersAdded_EmitsGameStartedEvent()
    {
        // arrange
        var (game, ticker, _, dispatcher) = CreateHarness();


        await game.AddPlayer(Guid.NewGuid(), "TestPlayer1");
        await game.AddPlayer(Guid.NewGuid(), "TestPlayer2");

        // act
        _ = Task.Run(() => game.Run());

        await ticker.Tick();

        // assert
        await dispatcher.Verify<GameStartedEvent>(game.Id);
    }
}