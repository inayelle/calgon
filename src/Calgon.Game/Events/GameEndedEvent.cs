namespace Calgon.Game;

internal sealed class GameEndedEvent : IGameEvent
{
    public required Player Winner { get; init; }
}