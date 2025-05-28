namespace Calgon.Game;

public sealed class GameStartedEvent : IGameEvent
{
    public required int MapSize { get; init; }
    public required TimeSpan TickPeriod { get; init; }

    public required IReadOnlyDictionary<Guid, Planet> Planets { get; init; }
    public required IReadOnlyDictionary<Guid, Player> Players { get; init; }
}