namespace Calgon.Game;

public sealed class Player : IEquatable<Player>
{
    public Guid Id { get; }
    public string? Name { get; }
    public int Color { get; }

    public Player(Guid id, string? name, int color)
    {
        Id = id;
        Name = name;
        Color = color;
    }

    public bool Equals(Player? other)
    {
        return other?.Id == Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is Player other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}