using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelCreationDto
{
    [Required] public string Thumbnail { get; set; }
    [Required] public string Owner { get; set; }
    [Required] public string Name { get; set; }
    [Required] public int CityId { get; set; }
    [Required] public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

}