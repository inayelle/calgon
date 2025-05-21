namespace Calgon.Host.Controllers.Rooms.Models;
public sealed class CreateRoomModel
{
    public required string Name { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Room name cannot be empty.");
        }

        if (Name.Length < 3 || Name.Length > 50)
        {
            throw new ArgumentException("Room name must be between 3 and 50 characters.");
        }
    }
}

internal sealed class RoomCreatedModel
{
    public required string InvitationCode { get; init; }
    public required string RoomId { get; init; }
}