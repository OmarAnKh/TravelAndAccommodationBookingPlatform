using Stripe;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IPaymentService
{

    Task<string> CreatePaymentIntentAsync(ReservationCreationDto reservationCreationDto, string userId);
    Task<(bool Success, string Message)> HandleStripeEventAsync(Event stripeEvent);
}