namespace Calgon.Game;

public interface IGameEventDispatcher
{
    Task Dispatch(Guid gameId, IEnumerable<IGameEvent> events);
}