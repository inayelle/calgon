using Calgon.Game;
using Calgon.Host.Game.Client;
using Calgon.Host.Game.Client.Args;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Game;

public sealed class GameHub : Hub<IGameHubClient>, IGameEventDispatcher
{
    async Task IGameEventDispatcher.Dispatch(Guid gameId, IEnumerable<IGameEvent> events)
    {
        var client = Clients.Group(gameId.ToString());

        foreach (var @event in events)
        {
            // TODO: replace with a kind of dynamic dispatch
            var task = @event switch
            {
                GameStartedEvent gameStarted => SendEvent(client, gameStarted),
                GameEndedEvent gameEnded => SendEvent(client, gameEnded),
                FleetSentEvent fleetSent => SendEvent(client, fleetSent),
                ShipsProducedEvent shipsProduced => SendEvent(client, shipsProduced),
                PlayerEliminatedEvent playerEliminated => SendEvent(client, playerEliminated),
                _ => Task.CompletedTask,
            };

            await task;
        }
    }

    private static Task SendEvent(IGameHubClient client, GameStartedEvent _)
    {
        return client.GameStarted(new GameStartedArgs());
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