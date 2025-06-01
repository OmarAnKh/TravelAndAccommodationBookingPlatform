namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewCreationDto
{
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}