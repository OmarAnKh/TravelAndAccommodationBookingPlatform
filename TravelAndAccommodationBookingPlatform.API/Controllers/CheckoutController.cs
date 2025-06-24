using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Handles payment-related operations using Stripe, including creating PaymentIntents and processing webhooks.
/// </summary>
[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<CheckoutController> _logger;
    /// <summary>
    /// Initializes a new instance of the <see cref="CheckoutController"/> class.
    /// </summary>
    /// <param name="paymentService">Service responsible for handling payment logic.</param>
    /// <param name="logger"></param>
    public CheckoutController(IPaymentService paymentService, ILogger<CheckoutController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a Stripe PaymentIntent for the specified reservation.
    /// </summary>
    /// <param name="reservationCreationDto">The reservation details required to calculate the amount.</param>
    /// <returns>
    /// Returns a client secret for the created PaymentIntent, or <see cref="UnauthorizedResult"/> if the user is not authenticated.
    /// </returns>
    /// <response code="200">Returns the client secret for Stripe payment.</response>
    /// <response code="401">User is not authorized.</response>
    [Authorize]
    [HttpPost("create-payment-intent")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] ReservationCreationDto reservationCreationDto)
    {
        try
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            var clientSecret = await _paymentService.CreatePaymentIntentAsync(reservationCreationDto, userId);
            return Ok(new { clientSecret });
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while creating payment intent");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Stripe webhook endpoint to process various payment-related events like PaymentIntent success, failure, etc.
    /// </summary>
    /// <remarks>
    /// Stripe sends events to this endpoint to notify your backend about the status of payments.
    /// </remarks>
    /// <returns>
    /// Returns <see cref="OkResult"/> if the event was handled successfully, otherwise returns <see cref="BadRequestResult"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown if WEBHOOKSECRET environment variable is missing.</exception>
    /// <response code="200">The event was successfully processed.</response>
    /// <response code="400">Invalid signature or processing failure.</response>
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook()
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var webhookSecret = Environment.GetEnvironmentVariable("WEBHOOKSECRET")
                                ?? throw new InvalidOperationException("Missing environment variable: WEBHOOKSECRET");

            var stripeSignature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }

            var (success, message) = await _paymentService.HandleStripeEventAsync(stripeEvent);
            return success ? Ok(message) : BadRequest(message);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occured while stripe webhook");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }
}