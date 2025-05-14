namespace Calgon.Host.Controllers.Rooms.Models;

internal sealed class CreateRoomModel
{
    public required int SeatsCount { get; init; }
}

internal sealed class RoomCreatedModel
{
    public required Guid Id { get; init; }
}