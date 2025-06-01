using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationDto
{
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float BookPrice { get; set; }
    public DateTime BookDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public BookingStatus BookingStatus { get; set; }
}