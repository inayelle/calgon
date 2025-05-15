using Calgon.Host.Hubs.Game.Client;
using Calgon.Host.Hubs.Game.Server;
using Calgon.Host.Hubs.Game.Server.Args;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Hubs.Game;

internal sealed class GameHub : Hub<IGameHubClient>, IGameHubServer
{
    public Task StartGame()
    {
        throw new NotImplementedException();
    }

    public Task SendFleet(SendFleetArgs args)
    {
        throw new NotImplementedException();
    }
}