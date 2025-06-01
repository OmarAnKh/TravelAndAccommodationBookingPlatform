namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Location;

public class LocationUpdateDto
{
    public int? HotelId { get; set; }
    public float? Longitude { get; set; }
    public float? Latitude { get; set; }
}