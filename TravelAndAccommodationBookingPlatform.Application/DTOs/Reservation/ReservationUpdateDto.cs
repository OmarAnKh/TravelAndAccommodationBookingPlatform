using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationUpdateDto
{
    [Required] public int UserId { get; set; }
    [Required] public int RoomId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public BookingStatus BookingStatus { get; set; }
}