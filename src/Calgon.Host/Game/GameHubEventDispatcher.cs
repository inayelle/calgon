using Calgon.Game;
using Calgon.Host.Game.Client;
using Calgon.Host.Game.Client.Args;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Game;

internal sealed class GameHubEventDispatcher : IGameEventDispatcher
{
    private readonly IHubContext<GameHub, IGameHubClient> _hubContext;
    private readonly ILogger<GameHubEventDispatcher> _logger;

    public GameHubEventDispatcher(
        IHubContext<GameHub, IGameHubClient> hubContext,
        ILogger<GameHubEventDispatcher> logger
    )
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Dispatch(Guid gameId, IReadOnlyCollection<IGameEvent> events)
    {
        var client = _hubContext.Clients.Group(gameId.ToString());

        foreach (var @event in events)
        {
            var task = @event switch
            {
                GameStartedEvent gameStarted => SendEvent(client, gameStarted),
                GameEndedEvent gameEnded => SendEvent(client, gameEnded),
                FleetSentEvent fleetSent => SendEvent(client, fleetSent),
                FleetArrivedEvent fleetArrived => SendEvent(client, fleetArrived),
                ShipsProducedEvent shipsProduced => SendEvent(client, shipsProduced),
                PlayerEliminatedEvent playerEliminated => SendEvent(client, playerEliminated),
                _ => Task.CompletedTask,
            };

            await task;
        }

        if (events.Count > 0)
        {
            _logger.LogInformation(
                "Dispatched game events. {GameId} {EventsCount}",
                gameId,
                events.Count
            );
        }
    }

    private static Task SendEvent(IGameHubClient client, GameStartedEvent @event)
    {
        return client.GameStarted(new GameStartedArgs
            {
                MapSize = @event.MapSize,
                TickPeriod = @event.TickPeriod,
                FleedSpeed = @event.FleetSpeed,
                Planets = @event
                    .Planets
                    .Values
                    .ToDictionary(
                        planet => planet.Id,
                        planet => new GameStartedArgs.PlanetItem
                        {
                            Id = planet.Id,
                            Location = planet.Location,
                            Size = planet.Size,
                            OwnerId = planet.Owner?.Id,
                            Ships = planet.Ships,
                        }
                    ),
                Players = @event
                    .Players
                    .Values
                    .ToDictionary(
                        player => player.Id,
                        player => new GameStartedArgs.PlayerItem
                        {
                            Id = player.Id,
                            Name = player.Name,
                            Color = player.Color,
                        }
                    ),
            }
        );
    }

    private static Task SendEvent(IGameHubClient client, GameEndedEvent @event)
    {
        return client.GameEnded(new GameEndedArgs
            {
                WinnerId = @event.Winner.Id,
            }
        );
    }

    private static Task SendEvent(IGameHubClient client, FleetSentEvent @event)
    {
        return client.FleetSent(new FleetSentArgs
            {
                Fleet = new FleetSentArgs.FleetItem
                {
                    Id = @event.Fleet.Id,
                    OwnerId = @event.Fleet.Owner.Id,
                    Ships = @event.Fleet.Ships,
                },
                DeparturePlanet = new FleetSentArgs.DeparturePlanetItem
                {
                    Id = @event.Fleet.DeparturePlanet.Id,
                    Ships = @event.Fleet.DeparturePlanet.Ships,
                },
                DestinationPlanet = new FleetSentArgs.DestinationPlanetItem
                {
                    Id = @event.Fleet.DestinationPlanet.Id,
                },
            }
        );
    }

    private static Task SendEvent(IGameHubClient client, FleetArrivedEvent @event)
    {
        return client.FleetArrived(new FleetArrivedArgs
            {
                FleetId = @event.Fleet.Id,
                DestinationPlanet = new FleetArrivedArgs.DestinationPlanetItem
                {
                    Id = @event.Fleet.DestinationPlanet.Id,
                    OwnerId = @event.Fleet.DestinationPlanet.Owner?.Id,
                    Ships = @event.Fleet.DestinationPlanet.Ships,
                },
            }
        );
    }

    private static Task SendEvent(IGameHubClient client, ShipsProducedEvent @event)
    {
        return client.ShipsProduced(new ShipsProducedArgs
            {
                PlanetId = @event.Planet.Id,
                Ships = @event.Planet.Ships,
            }
        );
    }

    private static Task SendEvent(IGameHubClient client, PlayerEliminatedEvent @event)
    {
        return client.PlayerEliminated(new PlayerEliminatedArgs
            {
                PlayerId = @event.Player.Id,
            }
        );
    }
}