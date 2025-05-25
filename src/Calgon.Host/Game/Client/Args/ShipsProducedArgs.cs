namespace Calgon.Host.Game.Client.Args;

public sealed class ShipsProducedArgs
{
    public required Guid PlanetId { get; init; }
    public required int Ships { get; init; }
}