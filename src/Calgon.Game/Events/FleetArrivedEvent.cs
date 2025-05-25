namespace Calgon.Game;

internal sealed class FleetArrivedEvent : IGameEvent
{
    public required Fleet Fleet { get; init; }
}