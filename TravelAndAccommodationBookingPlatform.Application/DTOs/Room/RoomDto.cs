using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Room;

public class RoomDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public RoomType RoomType { get; set; }
    public string? CustomRoomTypeName { get; set; }
    public float Price { get; set; }
    public string? Thumbnail { get; set; }
    public string? Description { get; set; }
    public string Availability { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int? RoomNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}