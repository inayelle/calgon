namespace Calgon.Game;

public sealed class PeriodicTimerGameTicker : IGameTicker
{
    private PeriodicTimer? _timer;

    public TimeSpan Period { get; }

    public PeriodicTimerGameTicker(TimeSpan period)
    {
        Period = period;
    }

    public ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        _timer ??= new PeriodicTimer(Period);

        return _timer.WaitForNextTickAsync(cancellationToken);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}