using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller for managing rooms in the Travel and Accommodation Booking Platform.
/// </summary>
[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoomController"/> class.
    /// </summary>
    /// <param name="roomService">The room service.</param>
    /// <param name="logger">The logger instance.</param>
    public RoomController(IRoomService roomService, ILogger<RoomController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of rooms based on the provided query parameters.
    /// </summary>
    /// <param name="roomQueryParameters">Filtering and pagination options.</param>
    /// <returns>A list of rooms with pagination metadata in the header.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms([FromQuery] RoomQueryParameters roomQueryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (rooms, metaData) = await _roomService.GetAllAsync(roomQueryParameters);
            Response.Headers.Append("Room-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(rooms);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while getting all rooms.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Creates a new room.
    /// </summary>
    /// <param name="roomCreationDto">Room creation details.</param>
    /// <param name="files">List of images (at least one thumbnail is required).</param>
    /// <returns>The created room.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomDto>> PostRoom([FromForm] RoomCreationDto roomCreationDto, List<IFormFile> files)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (files.Count == 0)
            {
                return BadRequest("At least one room media is required.");
            }

            var room = await _roomService.CreateAsync(roomCreationDto, files);
            if (room is null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetRoomById), new { roomId = room.Id }, room);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while creating room.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Retrieves a specific room by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to retrieve.</param>
    /// <returns>The requested room if found.</returns>
    [HttpGet("{roomId}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomDto>> GetRoomById(int roomId)
    {
        try
        {
            var room = await _roomService.GetByIdAsync(roomId);
            if (room is null)
            {
                return NotFound();
            }

            return Ok(room);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while getting room.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Applies a partial update to an existing room.
    /// </summary>
    /// <param name="roomId">The ID of the room to update.</param>
    /// <param name="patchDocument">A JSON Patch document describing the updates.</param>
    /// <returns>The updated room.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPatch]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomDto>> UpdateRoom(int roomId, JsonPatchDocument<RoomUpdateDto> patchDocument)
    {
        try
        {
            var updatedRoom = await _roomService.UpdateAsync(roomId, patchDocument);
            if (updatedRoom is null)
            {
                return NotFound();
            }

            return Ok(updatedRoom);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while updating room.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Deletes a specific room by its ID.
    /// </summary>
    /// <param name="roomId">The ID of the room to delete.</param>
    /// <returns>The deleted room.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpDelete("{roomId}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RoomDto>> DeleteRoom(int roomId)
    {
        try
        {
            var deletedRoom = await _roomService.DeleteAsync(roomId);
            if (deletedRoom is null)
            {
                return NotFound();
            }

            return Ok(deletedRoom);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while deleting room.");
            return StatusCode(500500, "An unexpected error occurred.");
        }
    }


    /// <summary>
    /// Get Room images path by roomId
    /// </summary>
    /// <param name="roomId">The ID of the room you want</param>
    /// <returns></returns>
    [HttpGet("api/room/{roomId}/images")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetRoomImages(int roomId)
    {
        try
        {
            var images = await _roomService.GetImagesPathAsync(roomId);
            return Ok(images);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unexpected error occurred while retrieving room images.");
            return StatusCode(500, "An unexpected error occurred while retrieving room images.");
        }
    }
    /// <summary>
    /// Retrieves available rooms for a specific hotel based on optional check-in/check-out dates and guest count.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel.</param>
    /// <returns>A list of available rooms that match the search criteria.</returns>
    [HttpGet("available/hotel/{hotelId}")]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms(int hotelId)
    {
        try
        {

            var availableRooms = await _roomService.GetAvailableRoomsAsync(hotelId);

            if (!availableRooms.Any())
            {
                return NotFound("No available rooms found.");
            }

            return Ok(availableRooms);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Unexpected error occurred while fetching available rooms.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}