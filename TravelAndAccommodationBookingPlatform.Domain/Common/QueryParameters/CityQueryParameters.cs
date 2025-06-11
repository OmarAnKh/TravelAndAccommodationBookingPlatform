namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class CityQueryParameters : IQueryParameters
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}