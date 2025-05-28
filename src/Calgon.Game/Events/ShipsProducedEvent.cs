namespace Calgon.Game;

public sealed class ShipsProducedEvent : IGameEvent
{
    public required Planet Planet { get; init; }
}