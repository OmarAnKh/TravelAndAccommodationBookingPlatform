namespace TravelAndAccommodationBookingPlatform.Application.DTOs.City;

public class CityUpdateDto
{
    public int CityId { get; set; }
    public string? Name { get; set; }

    public string? Country { get; set; }

    public string? PostOffice { get; set; }
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;


}