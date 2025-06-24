using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Room;

public class RoomCreationDto
{
    [Required] public int HotelId { get; set; }
    [Required] public RoomType RoomType { get; set; }
    public string? CustomRoomTypeName { get; set; }

    [Range(1, float.MaxValue, ErrorMessage = "The room price must be positive number.")]
    [Required]
    public float Price { get; set; }

    public string? Description { get; set; }
    public Availability Availability { get; set; } = Availability.Available;

    [Range(1, float.MaxValue, ErrorMessage = "The number of adults must be positive number.")]
    [Required]
    public int Adults { get; set; }

    [Range(1, float.MaxValue, ErrorMessage = "The number of children must be positive number.")]
    [Required]
    public int Children { get; set; }

    [Range(1, float.MaxValue, ErrorMessage = "The room number must be positive number.")]
    [Required]
    public int RoomNumber { get; set; }
}