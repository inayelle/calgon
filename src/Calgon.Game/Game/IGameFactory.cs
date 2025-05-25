namespace Calgon.Game;

public interface IGameFactory
{
    Game CreateGame(IEnumerable<Player> players);
}