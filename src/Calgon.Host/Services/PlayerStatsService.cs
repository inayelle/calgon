using Calgon.Host.Data;
using Calgon.Host.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calgon.Host.Services;

public sealed class PlayerStatsService
{
    private readonly ApplicationDbContext _dbContext;

    public PlayerStatsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddGame(IReadOnlyCollection<Guid> playerIds, Guid winnerId)
    {
        var playerStats = await GetPlayerStats(playerIds);

        foreach (var playerId in playerIds)
        {
            if (playerStats.TryGetValue(playerId, out var stats))
            {
                stats.AddGame(winnerId);
                continue;
            }

            stats = PlayerStats.Create(
                playerId,
                didWinFirstGame: playerId == winnerId
            );

            _dbContext.PlayerStats.Add(stats);
        }

        await _dbContext.SaveChangesAsync();
    }

    public Task<PlayerStats?> GetPlayerStats(Guid playerId)
    {
        return _dbContext
            .PlayerStats
            .AsNoTracking()
            .Where(stats => stats.PlayerId == playerId)
            .SingleOrDefaultAsync();
    }

    private Task<Dictionary<Guid, PlayerStats>> GetPlayerStats(IReadOnlyCollection<Guid> playerIds)
    {
        return _dbContext
            .PlayerStats
            .Where(stats => playerIds.Contains(stats.PlayerId))
            .ToDictionaryAsync(stats => stats.PlayerId);
    }
}