namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelUpdateDto
{
    public int HotelId { get; set; }
    public string? Name { get; set; }
    public int? CityId { get; set; }
    public string? Owner { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}