using System.Security.Claims;
using Calgon.Host.Data;
using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Data.Entities;
using Calgon.Host.Interfaces;
using Calgon.Host.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Calgon.Host.Tests;

public class RoomServiceTests
{
    private MockDb _db = new MockDb();
    private Mock<ICurrentUserService> _mockCurrentUser = null!;
    
    [SetUp]
    public void SetUp()
    {
        _mockCurrentUser = new Mock<ICurrentUserService>();
    }

    private void LogInAs(string userId)
    {
        _mockCurrentUser.Setup(m => m.CurrentUserId).Returns(userId);
    }

    [TestCase("123", "My room")]
    [TestCase("12345", "My room")]
    [TestCase("456", "Another room")]
    public async Task Test_CreateRoom(string userId, string name)
    {
        // Arrange
        LogInAs(userId);

        var roomService = new RoomService(_db.Context, _mockCurrentUser.Object, null!);

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
    
}