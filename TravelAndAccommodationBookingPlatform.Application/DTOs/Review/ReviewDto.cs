namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewDto
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}