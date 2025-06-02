namespace Calgon.Game.Tests.Game;

[TestOf(typeof(Calgon.Game.Game))]
public partial class GameTests
{
    [Test]
    public void AddPlayer_WhenPlayerIsNewAndGameIsIdle_AddsPlayer()
    {
        // arrange
        var (game, _, context, _) = CreateHarness();

        var playerProto = (Id: Guid.NewGuid(), Name: "TestPlayer1");

        // act & assert
        Assert.DoesNotThrowAsync(async () => await game.AddPlayer(playerProto.Id, playerProto.Name));

        Assert.That(context.Players, Does.ContainKey(playerProto.Id));
    }

    [Test]
    public async Task AddPlayer_WhenPlayerAlreadyAddedAndGameIsIdle_ThrowsInvalidOperationException()
    {
        // arrange
        var (game, _, _, _) = CreateHarness();

        var playerProto = (Id: Guid.NewGuid(), Name: "TestPlayer1");

        await game.AddPlayer(playerProto.Id, playerProto.Name);

        // act & assert
        Assert.ThrowsAsync<InvalidOperationException>(() => game.AddPlayer(playerProto.Id, playerProto.Name));
    }

    [Test]
    public async Task AddPlayer_WhenGameIsRunning_ThrowsInvalidOperationException()
    {
        // arrange
        var (game, ticker, context, _) = CreateHarness();

        var playerProto1 = (Id: Guid.NewGuid(), Name: "TestPlayer1");
        var playerProto2 = (Id: Guid.NewGuid(), Name: "TestPlayer2");
        var playerProto3 = (Id: Guid.NewGuid(), Name: "TestPlayer3");

        await game.AddPlayer(playerProto1.Id, playerProto1.Name);
        await game.AddPlayer(playerProto2.Id, playerProto2.Name);

        // act
        _ = Task.Run(() => game.Run());

        // 0th tick
        await ticker.Tick();

        // assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await game.AddPlayer(playerProto3.Id, playerProto3.Name)
        );

        Assert.That(context.Players, Does.Not.ContainKey(playerProto3.Id));
    }
}