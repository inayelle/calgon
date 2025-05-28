using Calgon.Host.Extensions;
using Calgon.Host.Game.Client;
using Calgon.Host.Game.Server;
using Calgon.Host.Game.Server.Args;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Game;

[Authorize]
public sealed class GameHub : Hub<IGameHubClient>, IGameHubServer
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public override async Task OnConnectedAsync()
    {
        if (!Context.GetHttpContext().TryGetRoomId(out var roomId))
        {
            throw new HubException("Missing room identifier.");
        }

        _gameService.TryAddGame(roomId);
        await _gameService.AddPlayer(roomId, Context.User!.GetUserId());

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        Context.Items["RoomId"] = roomId;
    }

    public Task StartGame()
    {
        if (!Context.GetHttpContext().TryGetRoomId(out var roomId))
        {
            throw new HubException("Missing room identifier.");
        }

        _gameService.StartGame(roomId);

        return Task.CompletedTask;
    }

    public async Task SendFleet(SendFleetArgs args)
    {
        if (!Context.GetHttpContext().TryGetRoomId(out var roomId))
        {
            throw new HubException("Missing room identifier.");
        }

        await _gameService.SendFleet(
            roomId,
            args.DeparturePlanetId,
            args.DestinationPlanetId,
            args.Portion
        );
    }
}

file static class Extensions
{
    public static bool TryGetRoomId(this HttpContext? httpContext, out Guid roomId)
    {
        if (httpContext is null || !httpContext.Request.Query.TryGetValue("roomId", out var roomIdText))
        {
            roomId = Guid.Empty;
            return false;
        }

        return Guid.TryParse(roomIdText, out roomId);
    }
}