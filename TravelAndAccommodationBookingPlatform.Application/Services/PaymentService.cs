using Stripe;
using TravelAndAccommodationBookingPlatform.Application.Common;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;


namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IReservationService _reservationService;

    public PaymentService(IReservationService reservationService)
    {

        _reservationService = reservationService;
    }
    public async Task<string> CreatePaymentIntentAsync(ReservationCreationDto reservationCreationDto, string userId)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(reservationCreationDto.BookPrice * 100),
            Currency = "usd",
            Metadata = reservationCreationDto.FromReservationCreationDtoToMetadata(userId),
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true,
                AllowRedirects = "never"
            }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return intent.ClientSecret;
    }

    public async Task<(bool Success, string Message)> HandleStripeEventAsync(Event stripeEvent)
    {
        switch (stripeEvent.Type)
        {
            case "payment_intent.created":
                try
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    var dto = intent.Metadata.FromMetadataReservationCreationDto();
                    var userId = intent.Metadata["userId"];

                    await _reservationService.CreateAsync(dto, Convert.ToInt32(userId));
                    return (true, "Reservation created successfully.");
                }
                catch (Exception ex)
                {
                    return (false, $"Failed to create reservation: {ex.Message}");
                }

            case "payment_intent.succeeded":
                try
                {
                    var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                    var dto = succeededIntent.Metadata.FromMetadataReservationCreationDto();
                    var userId = succeededIntent.Metadata["userId"];

                    var updated = await _reservationService.MarkAsPaidAsync(Convert.ToInt32(userId), Convert.ToInt32(dto.RoomId));
                    return updated
                        ? (true, $"Payment succeeded. Reservation marked as paid.")
                        : (false, $"Reservation not found.");
                }
                catch (Exception ex)
                {
                    return (false, $"Failed to mark payment as completed: {ex.Message}");
                }

            case "payment_intent.payment_failed":
                try
                {
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    var dto = failedIntent.Metadata.FromMetadataReservationCreationDto();
                    var userId = failedIntent.Metadata["userId"];
                    var updated = await _reservationService.MarkAsFailedAsync(Convert.ToInt32(userId), Convert.ToInt32(dto.RoomId));
                    return (false, $"Payment failed for reservation");
                }
                catch (Exception ex)
                {
                    return (false, $"Failed to process payment failure event: {ex.Message}");
                }
            case "charge.succeeded":
            case "charge.updated":
                return (true, $"Charge event received: {stripeEvent.Type}");
            default:
                return (false, $"Unhandled event type: {stripeEvent.Type}");
        }
    }

}