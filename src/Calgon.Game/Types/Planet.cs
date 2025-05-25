using System.Diagnostics.CodeAnalysis;

namespace Calgon.Game;

public sealed class Planet
{
    private const float BaseProductionRate = 0.1f;

    private float _ships;

    public Guid Id { get; }
    public Location Location { get; }
    public int Size { get; }

    public Player? Owner { get; private set; }
    public int Ships => (int)_ships;

    public bool Occupied => Owner is not null;

    public Planet(Location location, int size)
    {
        Id = Guid.NewGuid();
        Location = location;
        Size = size;
    }

    public void Accept(Player player, int ships)
    {
        Owner = player;

        _ships = ships;
    }

    public void Accept(Fleet fleet)
    {
        if (Owner is null)
        {
            Capture(fleet);
        }
        else if (Owner.Equals(fleet.Owner))
        {
            Reinforce(fleet);
        }
        else
        {
            Battle(fleet);
        }
    }

    public bool TrySendFleet(Planet destinationPlanet, float portion, [NotNullWhen(true)] out Fleet? fleet)
    {
        var ships = (int)MathF.Floor(Ships / portion);

        if (ships <= 0)
        {
            fleet = null;
            return false;
        }

        _ships -= ships;

        fleet = new Fleet(
            Owner!,
            this,
            destinationPlanet,
            ships
        );

        return true;
    }

    public void ProduceShips()
    {
        if (!Occupied)
        {
            return;
        }

        _ships += Size * BaseProductionRate;
    }

    private void Capture(Fleet fleet)
    {
        Owner = fleet.Owner;
        _ships = fleet.Ships;
    }

    private void Reinforce(Fleet fleet)
    {
        _ships += fleet.Ships;
    }

    private void Battle(Fleet fleet)
    {
        var diff = Ships - fleet.Ships;

        switch (diff)
        {
            case 0:
                Owner = null;
                _ships = 0;
                break;
            case < 0:
                _ships = -diff;
                Owner = fleet.Owner;
                break;
            case > 0:
                _ships = diff;
                break;
        }
    }
}