using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data;
using Calgon.Host.Data.Entities;
using Calgon.Host.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Calgon.Host.Services;

public class RoomService(ApplicationDbContext context, ICurrentUserService currentUser, UserManager<IdentityUser> userManager, IHubContext<RoomHub> hubContext) 
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
        if (await context.RoomMembers.AnyAsync(x => x.UserId == currentUser.CurrentUserId))
        {
            throw new InvalidOperationException("User is already in a room.");
        }

        var room = await context.Rooms.Include((r) => r.RoomMembers).FirstOrDefaultAsync(x => x.InvitationCode == invitationCode);
        if (room == null)
        {
            throw new ArgumentException("Room not found.");
        }
        
        if (room.Status != RoomStatus.Open)
        {
            throw new InvalidDataException("Room is not open.");
        }
        
        if (room.RoomMembers.Count >= 6) // TODO: move
        {
            throw new InvalidDataException("Room is full.");
        }

        var roomMember = new RoomMember
        {
            UserId = currentUser.CurrentUserId!,
            RoomId = room.Id,
        };

        context.RoomMembers.Add(roomMember);
        await context.SaveChangesAsync();

        await hubContext.Clients.Group(room.Id.ToString()).SendAsync("room_update", await GetInfo(room.Id));

        return room.Id;
    }

    public async Task<RoomDetailsModel> GetInfo(Guid roomId)
    {
        var room = await context.Rooms.Include((r) => r.RoomMembers).FirstOrDefaultAsync(x => x.Id == roomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found.");
        }

        // if user is not in the room
        if (!room.RoomMembers.Any(x => x.UserId == currentUser.CurrentUserId))
        {
            throw new InvalidOperationException("User is not in the room.");
        }

        // Fetch all users and perform the filtering in memory
        var roomMemberIds = room.RoomMembers
            .Select(x => x.UserId).ToList();
        var roomUsers = await userManager
            .Users
            .Where(u => roomMemberIds.Contains(u.Id)).ToListAsync();

        var roomMembers = roomUsers.Select(u => new RoomMemberModel
        {
            UserId = u!.Id,
            UserName = u.UserName ?? u.Email!,
        }).ToList();

        return new RoomDetailsModel
        {
            Name = room.Name,
            InvitationCode = room.InvitationCode,
            Status = room.Status,
            Members = roomMembers,
            RoomCreatorId = room.CreatedBy
        };
    }

    public async Task<Room> Leave(Guid roomId)
    {
        var room = await context.Rooms.Include((r) => r.RoomMembers).FirstOrDefaultAsync(x => x.Id == roomId);
        if (room == null)
        {
            throw new ArgumentException("Room not found.");
        }

        var roomMember = await context.RoomMembers.FirstOrDefaultAsync(x => x.UserId == currentUser.CurrentUserId && x.RoomId == roomId);
        if (roomMember == null)
        {
            throw new InvalidOperationException("User is not in the room.");
        }

        context.RoomMembers.Remove(roomMember);
        await context.SaveChangesAsync();
        
        await hubContext.Clients.Group(room.Id.ToString()).SendAsync("room_update", await GetInfo(room.Id));

        return room;
    }
    
}