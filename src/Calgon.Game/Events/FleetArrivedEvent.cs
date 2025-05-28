namespace Calgon.Game;

public sealed class FleetArrivedEvent : IGameEvent
{
    public required Fleet Fleet { get; init; }
}