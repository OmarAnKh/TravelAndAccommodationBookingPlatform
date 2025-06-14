using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.Review;

public class ReviewCreationDto
{
    [Required] public int UserId { get; set; }
    [Required] public int HotelId { get; set; }
    public string? Comment { get; set; }
    public float? Rate { get; set; }
    public string? ImagePath { get; set; }

}