using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class EndGamePipe : IGamePipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        if (context.Players.Count != 1)
        {
            next(context);
            return;
        }

        var winner = context.Players.Values.First();

        context.AddEvents(new GameEndedEvent
            {
                Winner = winner,
            }
        );

        next(context);
    }
}