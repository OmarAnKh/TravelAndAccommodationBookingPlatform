using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.City;

public class CityUpdateDto
{
    [Required] public int CityId { get; set; }
    public string? Name { get; set; }

    public string? Country { get; set; }

    public string? PostOffice { get; set; }


}