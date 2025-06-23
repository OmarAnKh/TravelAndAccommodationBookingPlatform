using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Handles operations related to reservations including creation, retrieval, deletion, and cancellation.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    private readonly IReservationService _reservationService;
    private readonly ILogger<ReservationController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReservationController"/> class.
    /// </summary>
    /// <param name="reservationService">Service for reservation operations.</param>
    /// <param name="logger">Logger for reservation controller.</param>
    public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger)
    {
        _reservationService = reservationService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all reservations based on provided query parameters.
    /// </summary>
    /// <param name="reservationQueryParameters">Filtering, sorting, and pagination parameters.</param>
    /// <returns>A list of reservations with pagination metadata in headers.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations([FromQuery] ReservationQueryParameters reservationQueryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var (reservation, metaData) = await _reservationService.GetAllAsync(reservationQueryParameters);
            Response.Headers.Append("User-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(reservation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while getting reservations.");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Retrieves a reservation by its unique ID.
    /// </summary>
    /// <param name="id">The reservation ID.</param>
    /// <returns>The reservation if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> GetReservationById(int id)
    {
        try
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation is null)
            {
                _logger.LogWarning("Reservation not found.");
                return NotFound();
            }
            return Ok(reservation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while getting reservation.");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Retrieves a reservation based on user ID and room ID.
    /// </summary>
    /// <param name="userId">The ID of the user who made the reservation.</param>
    /// <param name="roomId">The ID of the reserved room.</param>
    /// <returns>The reservation if found; otherwise, 404 Not Found.</returns>
    [HttpGet("user/{userId}/room/{roomId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservationDto>> GetReservationByRoomId(int userId, int roomId)
    {
        try
        {
            var reservation = await _reservationService.GetByUserAndRoomIdAsync(userId, roomId);
            if (reservation is null)
            {
                return NotFound();
            }
            return Ok(reservation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while getting reservation.");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Deletes a reservation by ID.
    /// </summary>
    /// <param name="id">The ID of the reservation to delete.</param>
    /// <returns>204 No Content if deleted successfully; 404 Not Found if not found.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteReservation(int id)
    {
        try
        {
            var reservation = await _reservationService.DeleteAsync(id);
            if (reservation is null)
            {
                return BadRequest();
            }
            return Ok(reservation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while deleting reservation.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Cancels a reservation by marking it as canceled.
    /// </summary>
    /// <param name="reservationId">The ID of the reservation to cancel.</param>
    /// <returns>204 No Content if canceled successfully; 404 Not Found if not found; 401 Unauthorized if user is not authenticated.</returns>
    [Authorize]
    [HttpGet("/Cancel/{reservationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CancelReservation(int reservationId)
    {
        try
        {
            var reservation = await _reservationService.MarkAsCancelledAsync(reservationId);
            if (!reservation)
            {
                return BadRequest();
            }
            return Ok(reservation);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while cancelling reservation.");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }
}