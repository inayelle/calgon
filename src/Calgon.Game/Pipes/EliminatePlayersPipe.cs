using AnyKit.Pipelines;

namespace Calgon.Game;

internal sealed class EliminatePlayersPipe : IGamePipe
{
    public void Invoke(GameContext context, Pipeline<GameContext> next)
    {
        var deadPlayers = context.Players.Values.ToHashSet();
        var alivePlayers = EnumeratePlayersWithAssets(context);
        deadPlayers.ExceptWith(alivePlayers);

        if (deadPlayers.Count == 0)
        {
            next(context);
            return;
        }

        foreach (var deadPlayer in deadPlayers)
        {
            context.Players.Remove(deadPlayer.Id);

            context.AddEvents(new PlayerEliminatedEvent
                {
                    Player = deadPlayer,
                }
            );
        }

        next(context);
    }

    private static IEnumerable<Player> EnumeratePlayersWithAssets(GameContext context)
    {
        foreach (var fleet in context.Fleets.Values)
        {
            yield return fleet.Owner;
        }

        foreach (var planet in context.Planets.Values)
        {
            if (planet.Owner is { } player)
            {
                yield return player;
            }
        }
    }
}