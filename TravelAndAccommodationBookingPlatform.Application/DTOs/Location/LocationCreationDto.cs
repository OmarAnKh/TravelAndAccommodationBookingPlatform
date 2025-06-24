using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Location;

public class LocationCreationDto
{
    [Required] public int HotelId { get; set; }
    [Required] public float Longitude { get; set; }
    [Required] public float Latitude { get; set; }
}