using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationUpdateDto
{
    public PaymentStatus PaymentStatus { get; set; }
    public BookingStatus BookingStatus { get; set; }
}