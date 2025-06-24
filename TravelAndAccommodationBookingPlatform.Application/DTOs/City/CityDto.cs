namespace TravelAndAccommodationBookingPlatform.Application.DTOs.City;

public class CityDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Thumbnail { get; set; }
    public string Country { get; set; }
    public string? PostOffice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}