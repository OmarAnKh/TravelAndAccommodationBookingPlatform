namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelDto
{
    public int Id { get; set; }
    public string Thumbnail { get; set; }
    public string Owner { get; set; }
    public string Name { get; set; }
    public int CityId { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}