using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data;
using Calgon.Host.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calgon.Host.Services;

public class RoomService(ApplicationDbContext context, CurrentUserService currentUser, UserManager<IdentityUser> userManager)
{
    public async Task<Room> Create(CreateRoomModel model)
    {
        if (await context.RoomMembers.AnyAsync(x => x.UserId == currentUser.CurrentUserId))
        {
            throw new InvalidOperationException("User is already in a room.");
        }
        
        var room = new Room
        {
            Name = model.Name,
            InvitationCode = Guid.NewGuid().ToString().Substring(0, 8),
            Status = RoomStatus.Open,
            CreatedBy = currentUser.CurrentUserId ?? throw new InvalidOperationException("Current user ID cannot be null.")
        };
        var roomMember = new RoomMember
        {
            UserId = currentUser.CurrentUserId,
            RoomId = room.Id,
        };

        context.Rooms.Add(room);
        context.RoomMembers.Add(roomMember);

        await context.SaveChangesAsync();

        return room;
    }
    public void GetRoom()
    {
        Console.WriteLine(currentUser.CurrentUserId);
    }
}