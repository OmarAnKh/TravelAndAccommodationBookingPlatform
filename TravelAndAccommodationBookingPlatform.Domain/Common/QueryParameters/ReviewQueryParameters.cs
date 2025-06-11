namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class ReviewQueryParameters : IQueryParameters
{

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? HotelId { get; set; }
    public int? UserId { get; set; }
    public float? Rating { get; set; }
}