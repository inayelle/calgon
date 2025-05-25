using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class LandFleetsPipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        var fleets = context.Fleets;

        var landedFleetIds = new List<Guid>(capacity: fleets.Count);

        foreach (var fleet in fleets.Values.Where(fleet => fleet.Arrived))
        {
            fleet.Land();

            landedFleetIds.Add(fleet.Id);
        }

        foreach (var landedFleetId in landedFleetIds)
        {
            fleets.Remove(landedFleetId);
        }

        next(context);
    }
}