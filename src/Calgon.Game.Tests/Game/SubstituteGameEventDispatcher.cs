using NSubstitute.Core;

namespace Calgon.Game.Tests.Game;

internal static class SubstituteGameEventDispatcher
{
    public static IGameEventDispatcher Create()
    {
        var dispatcher = Substitute.For<IGameEventDispatcher>();

        dispatcher
            .Dispatch(Arg.Any<Guid>(), Arg.Any<IReadOnlyCollection<IGameEvent>>())
            .Returns(Task.CompletedTask);

        return dispatcher;
    }

    public static WhenCalled<IGameEventDispatcher> When<TEvent>(this IGameEventDispatcher dispatcher, Guid gameId)
        where TEvent : class, IGameEvent
    {
        return dispatcher.When(x => x.Dispatch(
                Arg.Is(gameId),
                Arg.Is<IReadOnlyCollection<IGameEvent>>(events => events.OfType<TEvent>().Any())
            )
        );
    }

    public static void Then<TEvent>(
        this WhenCalled<IGameEventDispatcher> dispatcher,
        Action<IEnumerable<TEvent>> callback
    ) where TEvent : class, IGameEvent
    {
        dispatcher.Do(info => callback.Invoke(info.Arg<IReadOnlyCollection<IGameEvent>>().OfType<TEvent>()));
    }

    public static Task Verify<TEvent>(this IGameEventDispatcher dispatcher, Guid gameId)
        where TEvent : class, IGameEvent
    {
        return dispatcher
            .Received()
            .Dispatch(
                Arg.Is(gameId),
                Arg.Is<IReadOnlyCollection<IGameEvent>>(events => events.OfType<TEvent>().Any())
            );
    }

    public static Task Verify<TEvent>(this IGameEventDispatcher dispatcher, Guid gameId, int count)
        where TEvent : class, IGameEvent
    {
        return dispatcher
            .Received()
            .Dispatch(
                Arg.Is(gameId),
                Arg.Is<IReadOnlyCollection<IGameEvent>>(events => events.OfType<TEvent>().Count() == count)
            );
    }
}