using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calgon.Host.Controllers.Rooms;

[Route("rooms")]
internal sealed class RoomsController(RoomService service) : ApplicationController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RoomCreatedModel>> CreateRoom([FromBody] CreateRoomModel model)
    {
        try
        {
            model.Validate();
            var room = await service.Create(model);
            return Ok(new RoomCreatedModel
            {
                InvitationCode = room.InvitationCode
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpGet("join/{invitation_code}")]
    public async Task<ActionResult<RoomJoinedModel>> JoinRoom([FromRoute] string invitationCode)
    {
        if (string.IsNullOrWhiteSpace(invitationCode))
        {
            return BadRequest("Invitation code cannot be empty.");
        }

        try
        {
            var roomId = await service.Join(invitationCode);
            return Ok(new RoomJoinedModel
            {
                RoomId = roomId
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (InvalidDataException ex)
        {
            return BadRequest(ex.Message);
        }
    }



    [HttpGet]
    [Authorize]
    public FilteredRoomsModel GetRooms([FromQuery] string? searchPhrase = null)
    {
        service.GetRoom();
        throw new NotImplementedException();
    }
}