namespace Calgon.Game;

public interface IGameFactory
{
    Game CreateGame(IGameEventDispatcher gameEventDispatcher, IEnumerable<Player> players);
}