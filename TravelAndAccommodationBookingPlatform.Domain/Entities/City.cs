using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class City
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string? Thumbnail { get; set; }
    [Required] public string Country { get; set; }
    public string? PostOffice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<Hotel> Hotels { get; set; }

    public City()
    {
        Hotels = new List<Hotel>();
    }
}