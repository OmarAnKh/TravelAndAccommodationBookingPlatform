using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Location;

public class LocationUpdateDto
{
    [Required] public int HotelId { get; set; }
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
}