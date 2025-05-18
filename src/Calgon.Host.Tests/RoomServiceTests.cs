using System.Security.Claims;
using Calgon.Host.Data;
using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data.Entities;
using Calgon.Host.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Calgon.Host.Tests;

public class RoomServiceTests
{
    private Mock<ApplicationDbContext> _mockContext = null!; 
    private Mock<DbSet<Room>> _mockRoomDbSet = null!;
    private Mock<DbSet<RoomMember>> _mockRoomMemberDbSet = null!;
    
    private Mock<CurrentUserService> _mockCurrentUser = null!;
    
    private static readonly IQueryable<Room> RoomData = new List<Room>()
    {
        new()
        {
            Id = Guid.NewGuid(),
            Status = RoomStatus.Open,
            CreatedBy = "1",
            Name = "Test Room",
            
        }
    }.AsQueryable();
    
    private static readonly IQueryable<RoomMember> RoomMemberData = new List<RoomMember>()
    {
        new()
        {
            Id = Guid.NewGuid(),
            RoomId = RoomData.First().Id,
            UserId = "1"
        }
    }.AsQueryable();
    
    [SetUp]
    public void SetUp()
    {
        _mockRoomDbSet = new Mock<DbSet<Room>>();
        
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(RoomData.Provider);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(RoomData.Expression);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(RoomData.ElementType);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(() => RoomData.GetEnumerator());
        
        _mockRoomMemberDbSet = new Mock<DbSet<RoomMember>>();
        
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.Provider).Returns(RoomMemberData.Provider);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.Expression).Returns(RoomMemberData.Expression);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.ElementType).Returns(RoomMemberData.ElementType);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.GetEnumerator()).Returns(() => RoomMemberData.GetEnumerator());

        _mockContext = new Mock<ApplicationDbContext>();
        _mockContext.Setup(m => m.Rooms).Returns(_mockRoomDbSet.Object);
        _mockContext.Setup(m => m.RoomMembers).Returns(_mockRoomMemberDbSet.Object);
        
        _mockCurrentUser = new Mock<CurrentUserService>();
    }

    private void LogInAs(string userId)
    {
        _mockCurrentUser.Setup(m => m.CurrentUserId).Returns(userId);
    }

    [TestCase("123", "My room")]
    public async Task Test_CreateRoom(string userId, string name)
    {
        // Arrange
        LogInAs(userId);

        var roomService = new RoomService(_mockContext.Object, _mockCurrentUser.Object, null!);

        var model = new CreateRoomModel
        {
            Name = name
        };

        // Act
        var result = await roomService.Create(model);

        // Assert
        var room = RoomData.FirstOrDefault(x => x.Name == name && x.CreatedBy == userId);
        Assert.That(room, Is.Not.Null);
        Assert.That(RoomMemberData.Where(x => x.UserId == userId && x.RoomId == room.Id).Any(), Is.True);
        Assert.That(result.InvitationCode == room.InvitationCode);
    }
    
}