namespace Calgon.Game;

internal sealed class ShipsProducedEvent : IGameEvent
{
    public required Planet Planet { get; init; }
}