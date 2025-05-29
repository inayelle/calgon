using Calgon.Host.Game.Client;
using Calgon.Host.Game.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Calgon.Host.Services;


public class RoomHub : Hub<IGameHubClient>
{
    public override async Task OnConnectedAsync()
    {
        if (!Context.GetHttpContext().TryGetRoomId(out var roomId))
        {
            throw new HubException("Missing room identifier.");
        }
        
        Console.WriteLine(roomId + " " + Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        Context.Items["RoomId"] = roomId;
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