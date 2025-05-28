namespace Calgon.Game;

public sealed class GameContext
{
    private readonly Dictionary<Guid, Player> _players;
    private readonly Dictionary<Guid, Planet> _planets;
    private readonly Dictionary<Guid, Fleet> _fleets;

    private readonly List<IGameEvent> _eventBuffer;

    public Guid Id { get; }
    public int MapSize { get; }

    public IReadOnlyDictionary<Guid, Player> Players => _players;
    public IReadOnlyDictionary<Guid, Planet> Planets => _planets;
    public IReadOnlyDictionary<Guid, Fleet> Fleets => _fleets;

    public GameContext(
        Guid gameId,
        int mapSize,
        IEnumerable<Planet> planets
    )
    {
        Id = gameId;

        MapSize = mapSize;

        _planets = planets.ToDictionary(planet => planet.Id);
        _players = new Dictionary<Guid, Player>(capacity: 6);
        _fleets = new Dictionary<Guid, Fleet>(capacity: 32);

        _eventBuffer = new List<IGameEvent>(capacity: 32);
    }

    public bool TryAddPlayer(Player player)
    {
        if (_players.ContainsKey(player.Id))
        {
            return false;
        }

        if (_players.Count >= _players.Capacity)
        {
            return false;
        }

        _players[player.Id] = player;

        var vacantPlanet = _planets
            .Values
            .FirstOrDefault(planet => planet.Owner is null);

        if (vacantPlanet is null)
        {
            return false;
        }

        vacantPlanet.Accept(player, ships: 50);

        return true;
    }

    public void RemovePlayer(Player player)
    {
        _players.Remove(player.Id);
    }

    public void AddFleet(Fleet fleet)
    {
        _fleets[fleet.Id] = fleet;
    }

    public void RemoveFleet(Fleet fleet)
    {
        _fleets.Remove(fleet.Id);
    }

    public void AddEvents(params IEnumerable<IGameEvent> events)
    {
        _eventBuffer.AddRange(events);
    }

    public IGameEvent[] FlushEvents()
    {
        var events = _eventBuffer.ToArray();

        _eventBuffer.Clear();

        return events;
    }

    public bool PlayerHasAssets(Player player)
    {
        foreach (var fleet in _fleets.Values)
        {
            if (fleet.Owner.Id == player.Id)
            {
                return true;
            }
        }

        foreach (var planet in _planets.Values)
        {
            if (planet.Owner?.Id == player.Id)
            {
                return true;
            }
        }

        return false;
    }
}