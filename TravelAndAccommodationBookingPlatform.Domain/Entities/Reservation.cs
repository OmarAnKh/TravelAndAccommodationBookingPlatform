using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Reservation
{
    public int Id { get; set; }
    public User User { get; set; }
    [Required] public int UserId { get; set; }
    public Room Room { get; set; }
    [Required] public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public float BookPrice { get; set; }
    public DateTime BookDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public BookingStatus BookingStatus { get; set; } = BookingStatus.Pending;
}