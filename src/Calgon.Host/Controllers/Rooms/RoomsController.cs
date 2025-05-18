using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calgon.Host.Controllers.Rooms;

[Route("rooms")]
internal sealed class RoomsController(RoomService service) : ApplicationController
{
    [HttpGet]
    public FilteredRoomsModel GetRooms([FromQuery] string? searchPhrase = null)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public RoomCreatedModel CreateRoom([FromBody] CreateRoomModel model)
    {
        throw new NotImplementedException();
    }
}