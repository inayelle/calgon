namespace Calgon.Host.Hubs.Game.Client.Args;

public sealed class PlanetShipsProducedArgs
{
    public required IReadOnlyCollection<PlanetItem> Planets { get; init; }

    public sealed class PlanetItem
    {
        public required Guid Id { get; init; }
        public required int Ships { get; init; }
    }
}