using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewUpdateDto
{
    [Required] public int ReviewId { get; set; }
    public string? Comment { get; set; }
    public float? Rate { get; set; }
}