using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationCreationDto
{
    [Required] public int RoomId { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Required] public float BookPrice { get; set; }
}