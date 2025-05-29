using Calgon.Game;

internal sealed class DefaultMapGenerator : IMapGenerator
{
    private const int MapSize = 300;

    private const int PlanetsCapacity = 18;
    private const int PlanetMinSize = 1;
    private const int PlanetMaxSize = 3;
    private const int GridSize = 6; // 6x6 = 36 "zones"
    private const int MinDistance = 10; // Min distance between planet centers

    private static readonly Random Random = Random.Shared;

    public Map Generate()
    {
        var cellSize = MapSize / GridSize;

        var zoneIndices = Enumerable.Range(0, GridSize * GridSize).ToList();
        Shuffle(zoneIndices); // Shuffle "zones"

        var planets = new List<Planet>(capacity: PlanetsCapacity);

        foreach (var index in zoneIndices)
        {
            if (planets.Count >= PlanetsCapacity)
                break;

            var size = Random.Next(PlanetMinSize, PlanetMaxSize + 1);

            var gx = index % GridSize;
            var gy = index / GridSize;

            // "zone" center + random offset
            var baseX = gx * cellSize + cellSize / 2;
            var baseY = gy * cellSize + cellSize / 2;

            var offsetX = Random.Next(-cellSize / 4, cellSize / 4);
            var offsetY = Random.Next(-cellSize / 4, cellSize / 4);

            var x = Clamp(baseX + offsetX, size, MapSize - size);
            var y = Clamp(baseY + offsetY, size, MapSize - size);

            var location = new Location(x, y);

            // Check distance between planets
            if (planets.All(p => Distance(p.Location, location) >= MinDistance + size + p.Size))
            {
                planets.Add(new Planet(location, size));
            }
        }

        return new Map(MapSize, planets);
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static int Clamp(int value, int min, int max)
        => Math.Min(Math.Max(value, min), max);

    private static double Distance(Location a, Location b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
