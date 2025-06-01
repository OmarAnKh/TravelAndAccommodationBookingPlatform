namespace TravelAndAccommodationBookingPlatform.Application.DTOs.City;

public class CityCreationDto
{
    public string Name { get; set; }
    public string? Thumbnail { get; set; }
    public string Country { get; set; }
    public string? PostOffice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

}