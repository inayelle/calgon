using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data.Entities;
using Calgon.Host.Interfaces;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Calgon.Host.Tests;

public class RoomServiceTests
{
    private readonly MockDb _db = new MockDb();
    private Mock<ICurrentUserService> _mockCurrentUser = null!;
    private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
    
    [SetUp]
    public void SetUp()
    {
        _mockCurrentUser = new Mock<ICurrentUserService>();

        _mockUserManager = MockUserManager();
    }

    [TearDown]
    public void TearDown()
    {
        _db.Context.Database.EnsureDeleted();
        _db.Context.Database.EnsureCreated();
    }
    
    public static Mock<UserManager<IdentityUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        var mgr = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        
        mgr.Setup(umm => umm.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());

        return mgr;
    }

    private void LogInAs(string userId)
    {
        _mockCurrentUser.Setup(m => m.CurrentUserId).Returns(userId);
    }

    #region Create

    [TestCase("123", "My room")]
    [TestCase("12345", "My room")]
    [TestCase("456", "Another room")]
    public async Task Create_UserIsNotInARoom_ReturnsRoomWithInvitationCode(string userId, string name)
    {
        // Arrange
        LogInAs(userId);

        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);

        var model = new CreateRoomModel
        {
            Name = name
        };

        // Act
        var result = await roomService.Create(model);

        // Assert
        var room = _db.Context.Rooms.FirstOrDefault(x => x.Name == name && x.CreatedBy == userId);
        Assert.That(room, Is.Not.Null);
        Assert.That(_db.Context.RoomMembers.Any(x => x.UserId == userId && x.RoomId == room.Id), Is.True);
        Assert.That(result.InvitationCode, Is.EqualTo(room.InvitationCode));
    }
    
    [TestCase("123", "My room")]
    [TestCase("12345", "My room")]
    public async Task Create_UserIsAlreadyInARoom_ThrowsException(string userId, string name)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Name = name,
        };
        var roomMember = new RoomMember()
        {
            UserId = userId,
            RoomId = room.Id
        };
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.Add(roomMember);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        var model = new CreateRoomModel
        {
            Name = name
        };
        
        // Act
        // Assert
        Assert.ThrowsAsync(Is.InstanceOf<Exception>(), async () => await roomService.Create(model));
    }
    
    #endregion


    #region Join
    
    [TestCase("123", "AABB")]
    public async Task Join_UserNotInRoom_InvitationCodeValid_ReturnsRoomId(string userId, string invitationCode)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Name = "My room",
            InvitationCode = invitationCode
        };
        _db.Context.Rooms.Add(room);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        var result = await roomService.Join(invitationCode);
        
        Assert.That(result, Is.EqualTo(room.Id));
        Assert.That(await _db.Context.RoomMembers.AnyAsync(x => x.UserId == userId && x.RoomId == room.Id), Is.True);
    }

    [TestCase("123", "AABB")]
    public async Task Join_UserInRoom_ThrowsException(string userId, string invitationCode)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Name = "My room",
            InvitationCode = invitationCode
        };
        var roomMember = new RoomMember()
        {
            UserId = userId,
            RoomId = room.Id
        };
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.Add(roomMember);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Join(invitationCode));
    }

    [TestCase("123", "AABB")]
    public async Task Join_RoomIsFull_ThrowsException(string userId, string invitationCode)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Name = "My room",
            InvitationCode = invitationCode
        };
        var roomMembers = new List<RoomMember>();
        for (var i = 0; i < 6; i++)
        {
            roomMembers.Add(new RoomMember()
            {
                UserId = $"{i + 1}",
                RoomId = room.Id
            });
        }
        
            
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.AddRange(roomMembers);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Join(invitationCode));
    }

    [TestCase("123", "AABB")]
    public void Join_InvalidInvitationCode_ThrowsException(string userId, string invitationCode)
    {
        // Arrange
        LogInAs(userId);
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Join("InVaLiD"));
    }

    [TestCase("123", "AABB")]
    public async Task Join_RoomInGame_ThrowsException(string userId, string invitationCode)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Name = "My room",
            InvitationCode = invitationCode,
            Status = RoomStatus.InGame,
        };

        _db.Context.Rooms.Add(room);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Join(invitationCode));
    }
    
    #endregion


    #region Leave

    [TestCase("123", "12345678-1234-1234-1234-123456789012")]
    public async Task Leave_UserInRoom_Returns(string userId, string roomId)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Id = new Guid(roomId),
            Name = "My room",
        };
        var roomMember = new RoomMember()
        {
            UserId = userId,
            RoomId = room.Id
        };
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.Add(roomMember);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        await roomService.Leave(new Guid(roomId));
        
        // Assert
        Assert.That(await _db.Context.RoomMembers.AnyAsync(x => x.UserId == userId && x.Id == room.Id), Is.False);
    }

    [TestCase("123", "12345678-1234-1234-1234-123456789012")]
    public async Task Leave_RoomNotExist_ThrowsException(string userId, string roomId)
    {
        LogInAs(userId);
        
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = "My room",
        };
        
        _db.Context.Rooms.Add(room);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Leave(new Guid(roomId)));
    }

    [TestCase("123", "12345678-1234-1234-1234-012345678901")]
    public async Task Leave_UserNotInRoom_ThrowsException(string userId, string roomId)
    {
        // Arrange
        LogInAs(userId);
        var room = new Room
        {
            Id = new Guid(roomId),
            Name = "My room",
        };
        
        _db.Context.Rooms.Add(room);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.Leave(new Guid(roomId)));
    }

    #endregion


    #region GetInfo

    [TestCase("123")]
    public async Task GetInfo_ValidRoomId_ReturnsRoomInfoWithMembers(string userId)
    {
        // Arrange
        LogInAs(userId);

        var room = new Room()
        {
            Name = "My room",
            InvitationCode = "inv_code",
            Status = RoomStatus.InGame,
        };
        var roomMembers = new List<RoomMember>()
        {
            new RoomMember()
            {
                UserId = userId,
                RoomId = room.Id
            },
            new RoomMember()
            {
                UserId = "other_user",
                RoomId = room.Id,
            }
        };
        
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.AddRange(roomMembers);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        var info = await roomService.GetInfo(room.Id);
        
        // Assert
        Assert.That(info, Is.Not.Null);
        Assert.That(info.Members, Is.Not.Null);
        Assert.That(info.Members, Has.Count.EqualTo(roomMembers.Count));
        Assert.That(info.Status, Is.EqualTo(RoomStatus.InGame));
        Assert.That(info.InvitationCode, Is.EqualTo(room.InvitationCode));
        Assert.That(info.Name, Is.EqualTo(room.Name));
    }

    [TestCase("123", "12345678-1234-1234-1234-123456789012")]
    public async Task GetInfo_InvalidRoomId_ThrowsException(string userId, string roomId)
    {
        // Arrange
        LogInAs(userId);

        var room = new Room()
        {
            Name = "My room",
            InvitationCode = "inv_code",
            Status = RoomStatus.InGame,
        };
        var roomMembers = new List<RoomMember>()
        {
            new RoomMember()
            {
                UserId = userId,
                RoomId = room.Id
            },
            new RoomMember()
            {
                UserId = "other_user",
                RoomId = room.Id,
            }
        };
        
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.AddRange(roomMembers);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.GetInfo(Guid.NewGuid()));
    }

    [TestCase("123", "12345678-1234-1234-1234-123456789012")]
    public async Task GetInfo_UserNotInRoom_ThrowsException(string userId, string roomId)
    {
        // Arrange
        LogInAs(userId);

        var room = new Room()
        {
            Name = "My room",
            InvitationCode = "inv_code",
            Status = RoomStatus.InGame,
        };
        var roomMembers = new List<RoomMember>()
        {
            new RoomMember()
            {
                UserId = "another_user",
                RoomId = room.Id
            },
            new RoomMember()
            {
                UserId = "other_user",
                RoomId = room.Id,
            }
        };
        
        _db.Context.Rooms.Add(room);
        _db.Context.RoomMembers.AddRange(roomMembers);
        await _db.Context.SaveChangesAsync();
        
        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, _mockUserManager.Object);
        
        // Act
        // Assert
        Assert.CatchAsync(async () => await roomService.GetInfo(room.Id));
    }

    #endregion
}