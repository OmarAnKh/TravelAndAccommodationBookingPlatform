using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class RoomQueryParameters : IQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public int? HotelId { get; set; }
    public RoomType? RoomType { get; set; }
    public float? MinPrice { get; set; }
    public float? MaxPrice { get; set; }

    public int? Adults { get; set; }
    public int? Children { get; set; }
    public bool? AvailableOnly { get; set; }
}
