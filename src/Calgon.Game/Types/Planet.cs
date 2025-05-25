using System.Diagnostics.CodeAnalysis;

namespace Calgon.Game;

public sealed class Planet
{
    private const float BaseProductionRate = 0.02f;

    private float _productionProgress;

    public Guid Id { get; }
    public Location Location { get; }
    public int Size { get; }

    public Player? Owner { get; private set; }
    public int Ships { get; private set; }

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

        Ships = ships;
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

        Ships -= ships;

        fleet = new Fleet(
            Owner!,
            this,
            destinationPlanet,
            ships
        );

        return true;
    }

    public IEnumerable<IGameEvent> ProduceShips()
    {
        if (!Occupied)
        {
            yield break;
        }

        _productionProgress += BaseProductionRate;

        var ships = (int)MathF.Floor(_productionProgress);

        if (ships <= 0)
        {
            yield break;
        }

        _productionProgress -= ships;
        Ships += ships;

        yield return new ShipsProducedEvent
        {
            Planet = this,
        };
    }

    private void Capture(Fleet fleet)
    {
        Owner = fleet.Owner;
        Ships = fleet.Ships;
    }

    private void Reinforce(Fleet fleet)
    {
        Ships += fleet.Ships;
    }

    private void Battle(Fleet fleet)
    {
        var diff = Ships - fleet.Ships;

        switch (diff)
        {
            case 0:
                Owner = null;
                Ships = 0;
                break;
            case < 0:
                Ships = -diff;
                Owner = fleet.Owner;
                break;
            case > 0:
                Ships = diff;
                break;
        }
    }
}