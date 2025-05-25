using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class ProduceShipsPipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        foreach (var planet in context.Planets.Values)
        {
            var events = planet.ProduceShips();

            context.AddEvents(events);
        }

        next(context);
    }
}