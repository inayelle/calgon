namespace Calgon.Game;

public readonly struct Location
{
    public int X { get; }
    public int Y { get; }

    public Location(int x, int y)
    {
        X = x;
        Y = y;
    }

    public float DistanceTo(Location location)
    {
        var dx = X - location.X;
        var dy = Y - location.Y;

        return MathF.Sqrt(dx * dx + dy * dy);
    }
}