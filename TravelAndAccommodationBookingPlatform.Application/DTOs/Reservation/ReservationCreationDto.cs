using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationCreationDto
{
    [Required] public int UserId { get; set; }
    [Required] public int RoomId { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Required] public float BookPrice { get; set; }
    public DateTime BookDate { get; set; } = DateTime.Now;
}