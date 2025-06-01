namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewUpdateDto
{
    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}