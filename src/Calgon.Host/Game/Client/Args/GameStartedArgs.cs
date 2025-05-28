using Calgon.Game;

namespace Calgon.Host.Game.Client.Args;

public sealed class GameStartedArgs
{
    public required int MapSize { get; init; }
    public required TimeSpan TickPeriod { get; init; }

    public required IReadOnlyDictionary<Guid, PlanetItem> Planets { get; init; }
    public required IReadOnlyDictionary<Guid, PlayerItem> Players { get; init; }

    public sealed class PlanetItem
    {
        public required Guid Id { get; init; }
        public required Location Location { get; init; }

        public required int Size { get; init; }
        public required Guid? OwnerId { get; init; }

        public required int Ships { get; init; }
    }

    public sealed class PlayerItem
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}