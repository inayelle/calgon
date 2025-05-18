namespace Calgon.Host.Data.Entities;

public class RoomMember
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    
    public string UserId { get; set; } = string.Empty;
}