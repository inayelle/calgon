using Calgon.Host.Game.Server.Args;

namespace Calgon.Host.Game.Server;

public interface IGameHubServer
{
    Task StartGame();

    Task SendFleet(SendFleetArgs args);
}