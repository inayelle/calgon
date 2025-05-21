namespace Calgon.Host.Controllers.Rooms.Models;


public sealed class RoomMemberModel
{
    public required string UserId { get; init; }
    public required string UserName { get; init; }
}