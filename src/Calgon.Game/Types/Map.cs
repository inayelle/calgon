namespace Calgon.Game;

public sealed class Map
{
    public int Size { get; }
    public IReadOnlyCollection<Planet> Planets { get; }

    public Map(int size, IReadOnlyCollection<Planet> planets)
    {
        Size = size;
        Planets = planets;
    }
}