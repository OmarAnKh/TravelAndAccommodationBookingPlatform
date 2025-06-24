using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelUpdateDto
{
    public string? Name { get; set; }
    public int? CityId { get; set; }
    public string? Owner { get; set; }
    public string? Description { get; set; }
}