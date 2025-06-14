using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Room;

public class RoomCreationDto
{
    [Required] public int HotelId { get; set; }
    [Required] public RoomType RoomType { get; set; }
    public string? CustomRoomTypeName { get; set; }
    [Required] public float Price { get; set; }
    public string? Thumbnail { get; set; }
    public string? Description { get; set; }
    [Required] public string Availability { get; set; }
    [Required] public int Adults { get; set; }
    [Required] public int Children { get; set; }
    [Required] public int RoomNumber { get; set; }
}