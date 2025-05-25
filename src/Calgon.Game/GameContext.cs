namespace Calgon.Game;

public sealed class GameContext
{
    public Guid Id { get; }

    public Dictionary<Guid, Player> Players { get; }
    public Dictionary<Guid, Planet> Planets { get; }
    public Dictionary<Guid, Fleet> Fleets { get; }

    public List<IGameEvent> Events { get; }

    public GameContext(
        IEnumerable<Player> players,
        IEnumerable<Planet> planets
    )
    {
        Id = Guid.NewGuid();

        Players = players.ToDictionary(player => player.Id);
        Planets = planets.ToDictionary(planet => planet.Id);
        Fleets = new Dictionary<Guid, Fleet>(capacity: 32);

        Events = new List<IGameEvent>(capacity: 32);
    }
}