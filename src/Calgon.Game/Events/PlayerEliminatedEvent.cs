namespace Calgon.Game;

public sealed class PlayerEliminatedEvent : IGameEvent
{
    public required Player Player { get; init; }
}