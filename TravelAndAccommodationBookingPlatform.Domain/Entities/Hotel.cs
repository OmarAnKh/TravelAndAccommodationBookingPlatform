using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Hotel
{
    public int Id { get; set; }
    [Required] public string Thumbnail { get; set; }
    [Required] public string Owner { get; set; }
    [Required] public string Name { get; set; }
    public Location Location { get; set; }
    public City City { get; set; }
    [Required] public int CityId { get; set; }
    [Required] public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<Review> Reviews { get; set; }
    public IEnumerable<Room> Rooms { get; set; }
    public Hotel()
    {
        Reviews = new List<Review>();
        Rooms = new List<Room>();
    }
}