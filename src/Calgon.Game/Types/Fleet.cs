namespace Calgon.Game;

public sealed class Fleet
{
    private const float Speed = 0.05f;

    private float _distance;

    public Guid Id { get; }

    public Player Owner { get; }

    public Planet DeparturePlanet { get; }
    public Planet DestinationPlanet { get; }

    public bool Arrived => _distance < 0f;

    public int Ships { get; }

    public Fleet(
        Player owner,
        Planet departurePlanet,
        Planet destinationPlanet,
        int ships
    )
    {
        Id = Guid.NewGuid();

        Owner = owner;

        DeparturePlanet = departurePlanet;
        DestinationPlanet = destinationPlanet;

        Ships = ships;

        _distance = DeparturePlanet.Location.DistanceTo(DestinationPlanet.Location);
    }

    public void Advance()
    {
        _distance -= Speed;
    }

    public void Land()
    {
        DestinationPlanet.Accept(this);
    }
}