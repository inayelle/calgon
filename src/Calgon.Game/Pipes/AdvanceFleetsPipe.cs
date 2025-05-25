using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class AdvanceFleetsPipe : IGamePipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        foreach (var fleet in context.Fleets.Values)
        {
            fleet.Advance();
        }

        next(context);
    }
}