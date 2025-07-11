using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class Review
{
    [Key] public int ReviewId { get; set; }

    public User User { get; set; }
    [Required] public int UserId { get; set; }

    public Hotel Hotel { get; set; }
    [Required] public int HotelId { get; set; }

    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public string? FolderPath { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}