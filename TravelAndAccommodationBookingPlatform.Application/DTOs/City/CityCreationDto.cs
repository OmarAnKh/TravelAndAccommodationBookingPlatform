using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.City;

public class CityCreationDto
{
    [Required] public string Name { get; set; }
    [Required] public string Thumbnail { get; set; }
    [Required] public string Country { get; set; }
    public string? PostOffice { get; set; }


}