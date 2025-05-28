namespace Calgon.Game;

public sealed class GameContext
{
    private readonly List<IGameEvent> _events;

    public Guid Id { get; }

    public Dictionary<Guid, Player> Players { get; }
    public Dictionary<Guid, Planet> Planets { get; }
    public Dictionary<Guid, Fleet> Fleets { get; }

    public GameContext(
        IEnumerable<Player> players,
        IEnumerable<Planet> planets
    )
    {
        Id = Guid.NewGuid();

        Players = players.ToDictionary(player => player.Id);
        Planets = planets.ToDictionary(planet => planet.Id);
        Fleets = new Dictionary<Guid, Fleet>(capacity: 32);

        _events = new List<IGameEvent>(capacity: 32);
    }

    public void AddEvents(params IEnumerable<IGameEvent> events)
    {
        _events.AddRange(events);
    }

    public IGameEvent[] FlushEvents()
    {
        var events = _events.ToArray();

        _events.Clear();

        return events;
    }
}