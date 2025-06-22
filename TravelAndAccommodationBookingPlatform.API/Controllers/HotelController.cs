using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller for managing hotel-related operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class HotelController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ILogger<HotelController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HotelController"/> Class
    /// </summary>
    /// <param name="hotelService">The hotel service dependency for performing hotel operations.</param>
    /// <param name="logger">The logger service dependency for performing logging operations</param>
    public HotelController(IHotelService hotelService, ILogger<HotelController> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of hotels based on the provided query parameters.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>A list of <see cref="HotelDto"/> with pagination metadata in the response header.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels([FromQuery] HotelQueryParameters queryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (hotels, metaData) = await _hotelService.GetAllAsync(queryParameters);
            Response.Headers.Append("Hotel-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(hotels);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to retrieve hotels.");
            return StatusCode(500, "An unexpected error occurred while retrieving hotels.");
        }
    }

    /// <summary>
    /// Creates a new hotel with the provided data and thumbnails.
    /// </summary>
    /// <param name="hotelDto">The hotel creation data.</param>
    /// <param name="thumbnails">List of image files to use as hotel thumbnails (at least one is required).</param>
    /// <returns>The created hotel.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostHotel([FromForm] HotelCreationDto hotelDto, List<IFormFile> thumbnails)
    {
        try
        {
            if (thumbnails.Count == 0)
            {
                return BadRequest("At least one hotel thumbnail is required.");
            }
            var createdHotel = await _hotelService.CreateAsync(hotelDto, thumbnails);

            if (createdHotel == null)
            {
                return StatusCode(500, "Failed to create hotel.");
            }
            return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, createdHotel);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to create a new hotel.");
            return StatusCode(500, "An unexpected error occurred while creating the hotel.");
        }
    }

    /// <summary>
    /// Retrieves a hotel by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the hotel.</param>
    /// <returns>A <see cref="HotelDto"/></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HotelDto>> GetHotelById(int id)
    {
        try
        {
            var hotel = await _hotelService.GetByIdAsync(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to get hotel with ID {id}.");
            return StatusCode(500, "An unexpected error occurred while retrieving the hotel.");
        }
    }

    /// <summary>
    /// Applies a JSON patch to a hotel entity by ID.
    /// </summary>
    /// <param name="hotelId">The ID of the hotel you want to update</param>
    /// <param name="hotelDto">The JSON patch document describing changes to apply.</param>
    /// <returns>The updated <see cref="HotelDto"/>.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HotelDto>> PatchHotel(int hotelId, JsonPatchDocument<HotelUpdateDto> hotelDto)
    {
        try
        {
            var updatedHotel = await _hotelService.UpdateAsync(hotelId, hotelDto);
            if (updatedHotel == null)
            {
                return NotFound();
            }

            return Ok(updatedHotel);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to patch hotel with ID {hotelId}.");
            return StatusCode(500, "An unexpected error occurred while updating the hotel.");
        }
    }

    /// <summary>
    /// Deletes a hotel by its ID.
    /// </summary>
    /// <param name="id">The ID of the hotel to delete.</param>
    /// <returns>The deleted <see cref="HotelDto"/> if successful.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<HotelDto>> DeleteHotel(int id)
    {
        try
        {
            var deletedHotel = await _hotelService.DeleteAsync(id);
            if (deletedHotel == null)
            {
                return NotFound();
            }

            return Ok(deletedHotel);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to delete hotel with ID {id}.");
            return StatusCode(500, "An unexpected error occurred while deleting the hotel.");
        }
    }

    /// <summary>
    /// Get Hotel images path by hotelId
    /// </summary>
    /// <param name="hotelId">The ID of the hotel you want</param>
    /// <returns></returns>
    [HttpGet("api/hotels/{hotelId}/images")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetHotelImages(int hotelId)
    {
        try
        {
            var images = await _hotelService.GetImagesPathAsync(hotelId);
            return Ok(images);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to get images for hotel with ID {hotelId}.");
            return StatusCode(500, "An unexpected error occurred while retrieving hotel images.");
        }
    }
}