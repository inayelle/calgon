using Calgon.Host.Controllers.Stats.Models;
using Calgon.Host.Extensions;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calgon.Host.Controllers.Stats;

[Route("stats")]
internal sealed class StatsController : ApplicationController
{
    private readonly PlayerStatsService _playerStatsService;

    public StatsController(PlayerStatsService playerStatsService)
    {
        _playerStatsService = playerStatsService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<MyStatsModel> GetMyStats()
    {
        var stats = await _playerStatsService.GetPlayerStats(playerId: User.GetUserId());

        return new MyStatsModel
        {
            TotalGames = stats?.TotalGames ?? 0,
            WonGames = stats?.WonGames ?? 0,
        };
    }
}