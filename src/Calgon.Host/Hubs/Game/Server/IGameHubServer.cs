using Calgon.Host.Hubs.Game.Server.Args;

namespace Calgon.Host.Hubs.Game.Server;

public interface IGameHubServer
{
    Task StartGame();

    Task SendFleet(SendFleetArgs args);
}