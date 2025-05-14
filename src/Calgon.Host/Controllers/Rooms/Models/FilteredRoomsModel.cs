namespace Calgon.Host.Controllers.Rooms.Models;

internal sealed class FilteredRoomsModel
{
    public required IReadOnlyCollection<RoomItem> Rooms { get; init; }

    public sealed class RoomItem
    {
        public required Guid Id { get; init; }
        public required DateTime CreatedOn { get; init; }
    }
}