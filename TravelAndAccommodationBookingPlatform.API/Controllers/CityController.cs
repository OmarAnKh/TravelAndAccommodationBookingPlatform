using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller for managing city-related operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;
    private readonly ILogger<CityController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CityController"/> Class
    /// </summary>
    /// <param name="cityService">The city service dependency for performing city operations.</param>
    /// <param name="logger">The logger service dependency for performing logging operations.</param>
    public CityController(ICityService cityService, ILogger<CityController> logger)
    {
        _cityService = cityService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a paginated list of cities based on the provided query parameters.
    /// </summary>
    /// <param name="queryParameters">with pagination metadata in the response header.</param>
    /// <returns>A list of <see cref="CityDto"/> with pagination metadata in the response header.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CityDto>>> GetCities([FromQuery] CityQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var (cities, metaData) = await _cityService.GetAllAsync(queryParameters);
        Response.Headers.Append("Cities-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(cities);
    }

    /// <summary>
    /// Creates a new city with the provided data and files.
    /// </summary>
    /// <param name="cityDto">The hotel creation data.</param>
    /// <param name="files">List of files to use as hotel thumbnails (at least one is required).</param>
    /// <returns>The created city.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CityDto>> PostCity([FromForm] CityCreationDto cityDto, List<IFormFile> files)
    {
        if (files.Count == 0)
            return BadRequest("At least one hotel thumbnail is required.");

        var createdHotel = await _cityService.CreateAsync(cityDto, files);

        if (createdHotel == null)
            return StatusCode(500, "Failed to create hotel.");

        return CreatedAtAction(nameof(GetCityById), new { id = createdHotel.Id }, createdHotel);
    }

    /// <summary>
    /// Retrieves a city by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the city.</param>
    /// <returns>A <see cref="CityDto"/></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CityDto>> GetCityById(int id)
    {
        var city = await _cityService.GetByIdAsync(id);
        if (city is null)
        {
            _logger.LogCritical("Reservation not found.");
            return NotFound();
        }
        return Ok(city);
    }

    /// <summary>
    /// Applies a JSON patch to a hotel entity by ID.
    /// </summary>
    /// <param name="id">The ID of the city you want to update</param>
    /// <param name="patchDocument">The JSON patch document describing changes to apply</param>
    /// <returns>The updated <see cref="CityDto"/>.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto?>> PatchCity(int id, JsonPatchDocument<CityUpdateDto> patchDocument)
    {
        var updatedCity = await _cityService.UpdateAsync(id, patchDocument);
        if (updatedCity is null)
        {
            _logger.LogCritical("Reservation not found.");
            return BadRequest();
        }
        return Ok(updatedCity);
    }

    /// <summary>
    /// Deletes a city by its ID.
    /// </summary>
    /// <param name="id">The ID of the city to delete.</param>
    /// <returns>The deleted <see cref="CityDto"/> if successful.</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityDto>> DeleteCity(int id)
    {
        var deletedCity = await _cityService.DeleteAsync(id);
        if (deletedCity is null)
        {
            _logger.LogCritical("Reservation not found.");
            return BadRequest();
        }
        return Ok(deletedCity);
    }


    /// <summary>
    /// Get Cities images path by cityId
    /// </summary>
    /// <param name="hotelId">The ID of the city you want</param>
    /// <returns>A List<see cref="String"/> represents the images paths>/></returns>
    [HttpGet("api/hotels/{hotelId}/images")]
    public async Task<IActionResult> GetHotelImages(int hotelId)
    {
        var images = await _cityService.GetImagesPathAsync(hotelId);
        return Ok(images);
    }
}