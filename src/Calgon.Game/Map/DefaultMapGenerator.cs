namespace Calgon.Game;

internal sealed class DefaultMapGenerator : IMapGenerator
{
    private const int MapSize = 300;

    private const int PlanetsCapacity = 18;
    private const int PlanetMinSize = 1;
    private const int PlanetMaxSize = 3;

    private static readonly Random Random = Random.Shared;

    public Map Generate()
    {
        var planets = new List<Planet>(capacity: PlanetsCapacity);

        while (planets.Count < PlanetsCapacity)
        {
            var size = Random.Next(PlanetMinSize, PlanetMaxSize);

            var location = new Location(
                x: Random.Next(size, MapSize - size),
                y: Random.Next(size, MapSize - size)
            );

            planets.Add(new Planet(location, size));
        }

        return new Map(MapSize, planets);
    }
}