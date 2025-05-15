namespace Calgon.Host.Hubs.Game.Client.Args;

public sealed class FleetArrivedArgs
{
    public required Guid FleetId { get; init; }
    public required DestinationPlanetItem DestinationPlanet { get; init; }

    public sealed class DestinationPlanetItem
    {
        public required Guid Id { get; init; }
        public required Guid OwnerId { get; init; }
        public required int Ships { get; init; }
    }
}