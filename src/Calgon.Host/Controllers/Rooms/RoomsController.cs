using Calgon.Host.Controllers.Rooms.Models;
using Calgon.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
                InvitationCode = room.InvitationCode,
                RoomId = room.Id.ToString()
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

    [HttpGet("join/{invitationCode}")]
    [Authorize]
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

    [HttpGet("{roomId}")]
    [Authorize]
    public async Task<ActionResult<RoomDetailsModel>> GetRoom([FromRoute] Guid roomId)
    {
        if (roomId == Guid.Empty)
        {
            return BadRequest("Room ID cannot be empty.");
        }
        try
        {
            var roomInfo = await service.GetInfo(roomId);
            return Ok(roomInfo);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
        }
    }

    [HttpPost("/leave/{roomId}")]
    [Authorize]
    public async Task<ActionResult> LeaveRoom([FromRoute] Guid roomId)
    {
        // TODO: DELETE ROOM WHEN LAST USER OR HOST LEAVES
        if (roomId == Guid.Empty)
        {
            return BadRequest("Room ID cannot be empty.");
        }
        try
        {
            await service.Leave(roomId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}