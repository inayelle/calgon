using Calgon.Host.Game.Client.Args;

namespace Calgon.Host.Game.Client;

public interface IGameHubClient
{
    Task ShipsProduced(ShipsProducedArgs args);
}