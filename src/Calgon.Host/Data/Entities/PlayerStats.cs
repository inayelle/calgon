using System.ComponentModel.DataAnnotations;

namespace Calgon.Host.Data.Entities;

public sealed class PlayerStats
{
    [Key]
    public Guid PlayerId { get; private init; }

    public long TotalGames { get; private set; }
    public long WonGames { get; private set; }

    private PlayerStats()
    {
    }

    public static PlayerStats Create(Guid playerId, bool didWinFirstGame)
    {
        return new PlayerStats
        {
            PlayerId = playerId,
            TotalGames = 1,
            WonGames = didWinFirstGame ? 1 : 0,
        };
    }

    public void AddGame(Guid winnerId)
    {
        TotalGames++;

        if (PlayerId == winnerId)
        {
            WonGames++;
        }
    }
}