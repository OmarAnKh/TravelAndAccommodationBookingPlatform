using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Location
{
    public Hotel Hotel { get; set; }
    [Key] public int HotelId { get; set; }
    [Required] public float Longitude { get; set; }
    [Required] public float Latitude { get; set; }
}