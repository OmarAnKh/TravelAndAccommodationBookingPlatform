using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public Hotel Hotel { get; set; }
    public int HotelId { get; set; }
    public RoomType RoomType { get; set; }
    public string? CustomRoomTypeName { get; set; }
    public float Price { get; set; }
    public string FolderPath { get; set; }
    public string? Description { get; set; }
    public Availability Availability { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int? RoomNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<Reservation> Reservations { get; set; }
    public Room()
    {
        Reservations = new List<Reservation>();
    }
}