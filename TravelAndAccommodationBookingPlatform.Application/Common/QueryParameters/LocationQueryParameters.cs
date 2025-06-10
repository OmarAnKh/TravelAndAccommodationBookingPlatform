using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;

public class LocationQueryParameters : IQueryParameters
{

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? HotelId;
    public float? Latitude;
    public float? Longitude;
}