using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class ReviewQueryParameters : IQueryParameters
{

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 10, ErrorMessage = "PageSize must be between 1 and 10.")]
    public int PageSize { get; set; } = 10;

    public int? HotelId { get; set; }
    public int? UserId { get; set; }
    public float? Rating { get; set; }
}