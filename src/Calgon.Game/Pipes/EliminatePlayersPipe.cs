using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class EliminatePlayersPipe : IGamePipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        var deadPlayers = context.GetDeadPlayers();

        if (deadPlayers.Count == 0)
        {
            next(context);
            return;
        }

        var events = new List<PlayerEliminatedEvent>(capacity: deadPlayers.Count);

        foreach (var deadPlayer in deadPlayers)
        {
            context.RemovePlayer(deadPlayer);

            events.Add(new PlayerEliminatedEvent
                {
                    Player = deadPlayer,
                }
            );
        }

        context.AddEvents(events);

        next(context);
    }
}

file static class GameContextExtensions
{
    public static List<Player> GetDeadPlayers(this GameContext context)
    {
        var deadPlayers = new List<Player>();

        foreach (var player in context.Players.Values)
        {
            var hasAssets = context.PlayerHasAssets(player);

            if (!hasAssets)
            {
                deadPlayers.Add(player);
            }
        }

        return deadPlayers;
    }
}