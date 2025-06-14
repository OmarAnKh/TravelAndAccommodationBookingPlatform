using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Room;

public class RoomUpdateDto
{
    [Required] public int RoomId { get; set; }
    public string? CustomRoomTypeName { get; set; }
    public float Price { get; set; }
    public string? Description { get; set; }
    public string? Availability { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int? RoomNumber { get; set; }
}