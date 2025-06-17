using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;

public class HotelCreationDto
{
    [Required] public string Owner { get; set; }
    [Required] public string Name { get; set; }
    [Required] public int CityId { get; set; }
    [Required] public string Description { get; set; }


}