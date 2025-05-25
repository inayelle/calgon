namespace Calgon.Shared;

public static class SemaphoreSlimExtensions
{
    public static async Task<SemaphoreLease> Acquire(this SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();

        return new SemaphoreLease(semaphore);
    }

    public readonly struct SemaphoreLease : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public SemaphoreLease(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}