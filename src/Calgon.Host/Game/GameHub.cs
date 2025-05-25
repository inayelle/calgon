using Calgon.Game;
using Calgon.Host.Game.Client;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Game;

public sealed class GameHub : Hub<IGameHubClient>, IGameEventDispatcher
{
    async Task IGameEventDispatcher.Dispatch(Guid gameId, IEnumerable<IGameEvent> events)
    {
        var client = Clients.Group(gameId.ToString());

        foreach (var @event in events)
        {
            var task = @event switch
            {
                _ => Task.CompletedTask,
            };

            await task;
        }
    }
}