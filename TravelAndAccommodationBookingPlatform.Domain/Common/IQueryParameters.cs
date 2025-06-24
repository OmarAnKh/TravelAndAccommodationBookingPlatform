namespace TravelAndAccommodationBookingPlatform.Domain.Common;

public interface IQueryParameters
{
    int Page { get; set; }
    int PageSize { get; set; }
}