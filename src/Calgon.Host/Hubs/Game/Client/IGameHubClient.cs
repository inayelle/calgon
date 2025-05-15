using Calgon.Host.Hubs.Game.Client.Args;

namespace Calgon.Host.Hubs.Game.Client;

public interface IGameHubClient
{
    Task PlayerEliminated(PlayerEliminatedArgs args);

    Task GameStarted();
    Task GameEnded(GameEndedArgs args);

    Task FleetSent(FleetSentArgs args);
    Task FleetArrived(FleetArrivedArgs args);

    Task ShipsProduced(PlanetShipsProducedArgs args);
}