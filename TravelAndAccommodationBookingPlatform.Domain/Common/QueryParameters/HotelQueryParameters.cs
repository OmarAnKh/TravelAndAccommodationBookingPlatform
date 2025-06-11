namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class HotelQueryParameters : IQueryParameters
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool SortDescending { get; set; } = false;
}