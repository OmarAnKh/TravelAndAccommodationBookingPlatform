using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class RoomQueryParameters : IQueryParameters
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 10, ErrorMessage = "PageSize must be between 1 and 10.")]
    public int PageSize { get; set; } = 10;

    public int? HotelId { get; set; }
    public RoomType? RoomType { get; set; }
    public float? MinPrice { get; set; }
    public float? MaxPrice { get; set; }

    public int? Adults { get; set; }
    public int? Children { get; set; }
    public bool? AvailableOnly { get; set; }
}
