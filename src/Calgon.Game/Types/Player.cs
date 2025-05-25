namespace Calgon.Game;

public sealed class Player : IEquatable<Player>
{
    public Guid Id { get; init; }
    public string Name { get; init; }

    public Player(Guid id, string name)
    {
        Id = id;
        Name = name;
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