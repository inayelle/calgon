namespace Calgon.Game;

public sealed class GameEndedEvent : IGameEvent
{
    public required Player Winner { get; init; }
}