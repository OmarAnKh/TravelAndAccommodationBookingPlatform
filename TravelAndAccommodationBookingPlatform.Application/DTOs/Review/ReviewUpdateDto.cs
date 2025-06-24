using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewUpdateDto
{
    public string? Comment { get; set; }
    public float? Rate { get; set; }
}