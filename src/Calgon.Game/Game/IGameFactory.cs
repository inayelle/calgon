namespace Calgon.Game;

public interface IGameFactory
{
    Game CreateGame(Guid gameId, IGameEventDispatcher gameEventDispatcher);
}