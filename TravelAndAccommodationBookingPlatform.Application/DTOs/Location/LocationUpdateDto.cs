using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Location;

public class LocationUpdateDto
{
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
}