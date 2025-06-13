namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelCreationDto
{
    public string Thumbnail { get; set; }
    public string Owner { get; set; }
    public string Name { get; set; }
    public int CityId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}