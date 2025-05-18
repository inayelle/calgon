using System.Security.Claims;
using Calgon.Host.Data;
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
        
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.Provider).Returns(RoomMemberData.Provider);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.Expression).Returns(RoomMemberData.Expression);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.ElementType).Returns(RoomMemberData.ElementType);
        _mockRoomDbSet.As<IQueryable<Room>>().Setup(m => m.GetEnumerator()).Returns(() => RoomMemberData.GetEnumerator());
        
        _mockRoomMemberDbSet = new Mock<DbSet<RoomMember>>();
        
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.Provider).Returns(RoomData.Provider);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.Expression).Returns(RoomData.Expression);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.ElementType).Returns(RoomData.ElementType);
        _mockRoomMemberDbSet.As<IQueryable<RoomMember>>().Setup(m => m.GetEnumerator()).Returns(() => RoomData.GetEnumerator());

        _mockContext = new Mock<ApplicationDbContext>();
        _mockContext.Setup(m => m.Rooms).Returns(_mockRoomDbSet.Object);
        
        _mockCurrentUser = new Mock<CurrentUserService>();
    }

    private void LogInAs(string userId)
    {
        _mockCurrentUser.Setup(m => m.CurrentUserId).Returns(userId);
    }

    [TestCase("1", "My room", "")]
    public void Test_CreateRoom(string userId, string name, string expected)
    {
        
    }
    
}