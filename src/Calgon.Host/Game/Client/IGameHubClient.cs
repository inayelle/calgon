using Calgon.Host.Game.Client.Args;

namespace Calgon.Host.Game.Client;

public interface IGameHubClient
{
    Task GameStarted(GameStartedArgs args);
    Task GameEnded(GameEndedArgs args);

    Task FleetSent(FleetSentArgs args);
    Task FleetArrived(FleetArrivedArgs args);

    Task ShipsProduced(ShipsProducedArgs args);

    Task PlayerEliminated(PlayerEliminatedArgs args);
}