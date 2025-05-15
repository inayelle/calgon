namespace Calgon.Host.Hubs.Game.Client.Args;

public sealed class FleetSentArgs
{
    public required FleetItem Fleet { get; init; }
    public required DeparturePlanetItem DeparturePlanet { get; init; }
    public required DestinationPlanetItem DestinationPlanet { get; init; }

    public sealed class FleetItem
    {
        public required Guid Id { get; init; }
        public required Guid OwnerId { get; init; }
        public required int Ships { get; init; }
    }

    public sealed class DeparturePlanetItem
    {
        public required Guid Id { get; init; }
        public required int Ships { get; init; }
    }

    public sealed class DestinationPlanetItem
    {
        public required Guid Id { get; init; }
    }
}