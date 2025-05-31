using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Review
{

    public User User { get; set; }
    [Required] public int UserId { get; set; }
    public Hotel Hotel { get; set; }
    [Required] public int HotelId { get; set; }
    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}