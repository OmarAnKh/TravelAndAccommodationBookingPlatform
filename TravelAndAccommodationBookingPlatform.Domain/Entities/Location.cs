using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Location
{
    public int Id { get; set; }
    [Required] public float Longitude { get; set; }
    [Required] public float Latitude { get; set; }
}