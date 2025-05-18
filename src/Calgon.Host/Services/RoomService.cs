using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data;
using Calgon.Host.Data.Entities;
using Calgon.Host.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calgon.Host.Services;

public class RoomService(ApplicationDbContext context, ICurrentUserService currentUser, UserManager<IdentityUser> userManager)
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
            CreatedBy = currentUser.CurrentUserId!
        };
        var roomMember = new RoomMember
        {
            UserId = currentUser.CurrentUserId!,
            RoomId = room.Id,
        };

        context.Rooms.Add(room);
        context.RoomMembers.Add(roomMember);

        await context.SaveChangesAsync();

        return room;
    }

    public async Task<Guid> Join(string invitationCode)
    {
        var room = await context.Rooms.Include((r) => r.RoomMembers).FirstOrDefaultAsync(x => x.InvitationCode == invitationCode);
        if (room == null)
        {
            throw new ArgumentException("Room not found.");
        } else if (room.Status != RoomStatus.Open)
        {
            throw new InvalidDataException("Room is not open.");
        } else if (room.RoomMembers.Any(x => x.UserId == currentUser.CurrentUserId))
        {
            throw new InvalidOperationException("User is already in a room.");
        }

        if (await context.RoomMembers.AnyAsync(x => x.UserId == currentUser.CurrentUserId && x.RoomId != room.Id))
        {
            throw new InvalidOperationException("User is already in a room.");
        }

        var roomMember = new RoomMember
        {
            UserId = currentUser.CurrentUserId!,
            RoomId = room.Id,
        };

        context.RoomMembers.Add(roomMember);
        await context.SaveChangesAsync();

        return room.Id;
    }

    public void GetRoom()
    {
        Console.WriteLine(currentUser.CurrentUserId);
    }
}