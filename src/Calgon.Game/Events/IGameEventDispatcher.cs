namespace Calgon.Game;

public interface IGameEventDispatcher
{
    Task Dispatch(Guid gameId, IReadOnlyCollection<IGameEvent> events);
}