using Calgon.Host.Data.Entities;

namespace Calgon.Host.Controllers.Rooms.Models;

public sealed class RoomDetailsModel
{
    public required string Name { get; init; }
    public required string InvitationCode { get; init; }
    public required RoomStatus Status { get; init; }
    public required string RoomCreatorId { get; init; }
    public required List<RoomMemberModel> Members { get; init; } = new();
}