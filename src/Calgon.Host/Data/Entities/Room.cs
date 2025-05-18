namespace Calgon.Host.Data.Entities;

public class Room
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InvitationCode { get; set; } = string.Empty;
    public RoomStatus Status { get; set; }
    
    public ICollection<RoomMember> RoomMembers { get; set; } = null!;
}

public enum RoomStatus
{
    Open,
    InGame
}