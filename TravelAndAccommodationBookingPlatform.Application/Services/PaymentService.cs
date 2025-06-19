using Stripe;
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
        var metadata = new Dictionary<string, string>
        {
            { "userId", userId },
            { "roomId", reservationCreationDto.RoomId.ToString() },
            { "startDate", reservationCreationDto.StartDate.ToString("o") },
            { "endDate", reservationCreationDto.EndDate.ToString("o") },
            { "bookPrice", reservationCreationDto.BookPrice.ToString("F2") },
        };

        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(reservationCreationDto.BookPrice * 100),
            Currency = "usd",
            Metadata = metadata,
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
                    var metadata = intent.Metadata;

                    var dto = new ReservationCreationDto
                    {
                        RoomId = int.Parse(metadata["roomId"]),
                        StartDate = DateTime.Parse(metadata["startDate"]),
                        EndDate = DateTime.Parse(metadata["endDate"]),
                        BookPrice = float.Parse(metadata["bookPrice"]),
                    };
                    var userId = metadata["userId"];

                    var created = await _reservationService.CreateAsync(dto, Convert.ToInt32(userId));
                    if (created is null)
                    {
                        return (false, "Reservation already exists");
                    }


                    var updateOptions = new PaymentIntentUpdateOptions
                    {
                        Metadata = new Dictionary<string, string>(metadata)
                        {
                            ["reservationId"] = created.Id.ToString()
                        }
                    };

                    var service = new PaymentIntentService();
                    await service.UpdateAsync(intent.Id, updateOptions);

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
                    var metadata = succeededIntent.Metadata;

                    var reservationId = int.Parse(metadata["reservationId"]);

                    var updated = await _reservationService.MarkAsPaidAsync(reservationId);
                    return updated
                        ? (true, "Payment succeeded. Reservation marked as paid.")
                        : (false, "Reservation not found.");
                }
                catch (Exception ex)
                {
                    return (false, $"Failed to mark payment as completed: {ex.Message}");
                }

            case "payment_intent.payment_failed":
                try
                {
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    var metadata = failedIntent.Metadata;

                    var reservationId = int.Parse(metadata["reservationId"]);

                    await _reservationService.MarkAsFailedAsync(reservationId);
                    return (false, "Payment failed for reservation");
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