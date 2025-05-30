using Calgon.Game;

internal sealed class DefaultMapGenerator : IMapGenerator
{
    private const int MapSize = 300;

    private const int PlanetsCapacity = 18;
    private const int PlanetMinSize = 10;
    private const int PlanetMaxSize = 15;
    private const int GridSize = 6; // 6x6 = 36 "zones"
    private const int CellSize = MapSize / GridSize;
    private const int MinDistance = 10; // Min distance between planet centers

    private static readonly Random Random = Random.Shared;

    public Map Generate()
    {
        var zoneIndices = Enumerable.Range(0, GridSize * GridSize).ToArray();
        Random.Shuffle(zoneIndices); // Use built-in shuffle

        var planets = new List<Planet>(capacity: PlanetsCapacity);

        foreach (var index in zoneIndices)
        {
            if (planets.Count >= PlanetsCapacity)
                break;

            var size = Random.Next(PlanetMinSize, PlanetMaxSize + 1);

            var gx = index % GridSize;
            var gy = index / GridSize;

            // "zone" center + random offset
            var baseX = gx * CellSize + CellSize / 2;
            var baseY = gy * CellSize + CellSize / 2;

            var offsetX = Random.Next(-CellSize / 4, CellSize / 4);
            var offsetY = Random.Next(-CellSize / 4, CellSize / 4);

            var x = Math.Clamp(baseX + offsetX, size, MapSize - size);
            var y = Math.Clamp(baseY + offsetY, size, MapSize - size);

            var location = new Location(x, y);

            if (planets.All(p => p.Location.DistanceTo(location) >= MinDistance + size + p.Size))
            {
                planets.Add(new Planet(location, size));
            }
        }

        return new Map(MapSize, planets);
    }
}
