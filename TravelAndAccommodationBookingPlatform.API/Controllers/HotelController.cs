using System.Text.Json;
using Azure;
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
    private const int MaxPageSize = 10;
    /// <summary>
    /// Initializes a new instance of the <see cref="HotelController"/> class.
    /// </summary>
    /// <param name="hotelService">The hotel service dependency for performing hotel operations.</param>
    public HotelController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    /// <summary>
    /// Retrieves a paginated list of hotels based on the provided query parameters.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>A list of <see cref="HotelDto"/> with pagination metadata in the response header.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels([FromQuery] HotelQueryParameters queryParameters)
    {
        if (queryParameters.PageSize > MaxPageSize)
        {
            queryParameters.PageSize = MaxPageSize;
        }

        var (hotels, metaData) = await _hotelService.GetAllAsync(queryParameters);
        Response.Headers.Append("Hotel-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(hotels);

    }

    /// <summary>
    /// Creates a new hotel with the provided data and thumbnails.
    /// </summary>
    /// <param name="hotelDto">The hotel creation data.</param>
    /// <param name="thumbnails">List of image files to use as hotel thumbnails (at least one is required).</param>
    /// <returns>The created hotel and a 201 Created response, or an error status if creation fails.</returns>
    [HttpPost]
    public async Task<IActionResult> PostHotel([FromForm] HotelCreationDto hotelDto, List<IFormFile> thumbnails)
    {
        if (thumbnails.Count == 0)
            return BadRequest("At least one hotel thumbnail is required.");

        var createdHotel = await _hotelService.CreateAsync(hotelDto, thumbnails);

        if (createdHotel == null)
            return StatusCode(500, "Failed to create hotel.");

        return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, createdHotel);

    }

    /// <summary>
    /// Retrieves a hotel by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the hotel.</param>
    /// <returns>The <see cref="HotelDto"/> if found, otherwise a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetHotelById(int id)
    {
        var hotel = await _hotelService.GetByIdAsync(id);
        if (hotel == null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    /// <summary>
    /// Applies a JSON patch to a hotel entity by ID.
    /// </summary>
    /// <param name="id">The ID of the hotel to update.</param>
    /// <param name="hotelDto">The JSON patch document describing changes to apply.</param>
    /// <returns>The updated <see cref="HotelDto"/> if successful, or 404 if the hotel is not found.</returns>
    [HttpPatch]
    public async Task<ActionResult<HotelDto>> PatchHotel(int id, JsonPatchDocument<HotelUpdateDto> hotelDto)
    {
        var updatedHotel = await _hotelService.UpdateAsync(id, hotelDto);
        if (updatedHotel == null)
        {
            return NotFound();
        }
        return Ok(updatedHotel);
    }

    /// <summary>
    /// Deletes a hotel by its ID.
    /// </summary>
    /// <param name="id">The ID of the hotel to delete.</param>
    /// <returns>The deleted <see cref="HotelDto"/> if successful, or 404 if the hotel is not found.</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<HotelDto>> DeleteHotel(int id)
    {
        var deletedHotel = await _hotelService.DeleteAsync(id);
        if (deletedHotel == null)
        {
            return NotFound();
        }
        return Ok(deletedHotel);
    }
}