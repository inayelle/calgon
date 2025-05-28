namespace Calgon.Game;

public sealed class FleetSentEvent : IGameEvent
{
    public required Fleet Fleet { get; init; }
}