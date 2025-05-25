using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class LandFleetsPipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        var fleets = context.Fleets.Values.ToArray();

        foreach (var fleet in fleets)
        {
            if (!fleet.Arrived)
            {
                continue;
            }

            var events = fleet.Land();

            context.Events.AddRange(events);

            context.Fleets.Remove(fleet.Id);
        }

        next(context);
    }
}