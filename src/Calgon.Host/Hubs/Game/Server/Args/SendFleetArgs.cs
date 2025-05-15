namespace Calgon.Host.Hubs.Game.Server.Args;

public sealed class SendFleetArgs
{
    public required Guid DeparturePlanetId { get; init; }
    public required Guid DestinationPlanetId { get; init; }

    public required float Portion { get; init; }
}