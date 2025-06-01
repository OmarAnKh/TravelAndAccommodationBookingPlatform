namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

public class ReservationCreationDto
{
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float BookPrice { get; set; }
    public DateTime BookDate { get; set; }
}