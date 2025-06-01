namespace Calgon.Game;

public interface IGameTicker : IDisposable
{
    TimeSpan Period { get; }

    ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default);
}