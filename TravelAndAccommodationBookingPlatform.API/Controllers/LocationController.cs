using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller for managing location-related operations
/// </summary>
[ApiController]
[Route("[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationController> _logger;
    /// <summary>
    /// Initializes a new instance of the <see cref="LocationController"/> Class
    /// </summary>
    /// <param name="locationService">The location service dependency for performing location operation</param>
    /// <param name="logger">The logger service dependency for performing logging operations</param>
    public LocationController(ILocationService locationService, ILogger<LocationController> logger)
    {
        _locationService = locationService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve a paginated list of Locations based on the provided query parameters
    /// </summary>
    /// <param name="queryParameters"> Query parameters for filtering and pagination</param>
    /// <returns>A list of <see cref="LocationDto"/> with pagination metadata in the response header</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations([FromQuery] LocationQueryParameters queryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (locations, metaData) = await _locationService.GetAllAsync(queryParameters);
            Response.Headers.Append("Location-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(locations);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to get locations");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Creates a new location with the provided data
    /// </summary>
    /// <param name="locationDto">The location creation data</param>
    /// <returns>The created location</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LocationDto>> PostLocation(LocationCreationDto locationDto)
    {
        try
        {
            var createdLocation = await _locationService.CreateAsync(locationDto);
            if (createdLocation is null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.HotelId }, createdLocation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while adding new location.");
            return StatusCode(500, "An Unexpected error occured.");
        }

    }
    /// <summary>
    /// Retrieves a location by its unique ID
    /// </summary>
    /// <param name="hotelId">The ID of the location</param>
    /// <returns>A <see cref="LocationDto"/></returns>
    [HttpGet("{hotelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LocationDto>> GetLocationById(int hotelId)
    {
        try
        {
            var location = await _locationService.GetByIdAsync(hotelId);
            if (location is null)
            {
                return NotFound();
            }
            return Ok(location);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while getting location.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }


    /// <summary>
    /// Applies a JSON patch to a location entity by ID.
    /// </summary>
    /// <param name="locationId">The ID of the location you want to update</param>
    /// <param name="locationDto">The JSON patch document describing changes to apply</param>
    /// <returns>The updated <see cref="LocationDto"/></returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LocationDto>> PatchLocation(int locationId, JsonPatchDocument<LocationUpdateDto> locationDto)
    {
        try
        {
            var updatedLocation = await _locationService.UpdateAsync(locationId, locationDto);
            if (updatedLocation is null)
            {
                return BadRequest();
            }
            return Ok(updatedLocation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while patching location.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Delete a location by its ID.
    /// </summary>
    /// <param name="hotelId">The ID of the location to delete</param>
    /// <returns>The deleted<see cref="LocationDto"/> if successful</returns>
    [Authorize(Policy = "MustBeAnAdmin")]
    [HttpDelete("{hotelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LocationDto>> DeleteLocation(int hotelId)
    {
        try
        {
            var deletedLocation = await _locationService.DeleteAsync(hotelId);
            if (deletedLocation is null)
            {
                return NotFound();
            }
            return Ok(deletedLocation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while deleting location.");
            return StatusCode(500, "An unexpected error occurred.");
        }


    }

}