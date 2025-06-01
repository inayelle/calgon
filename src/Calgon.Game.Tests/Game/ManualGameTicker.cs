namespace Calgon.Game.Tests.Game;

internal sealed class ManualGameTicker : IGameTicker
{
    private readonly SemaphoreSlim _semaphoreSlim = new(initialCount: 0, maxCount: Int32.MaxValue);

    public TimeSpan Period { get; }

    public ManualGameTicker(TimeSpan period)
    {
        Period = period;
    }

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);

        return true;
    }

    public async Task Tick(int ticks = 1)
    {
        if (ticks < 1)
        {
            throw new ArgumentException("The times argument must be greater than zero.");
        }

        while (ticks-- > 0)
        {
            _semaphoreSlim.Release();

            await Task.Delay(Period);
        }
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
    }
}