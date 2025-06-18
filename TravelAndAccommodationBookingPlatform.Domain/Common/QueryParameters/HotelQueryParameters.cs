using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class HotelQueryParameters : IQueryParameters
{
    public string? SearchTerm { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 10, ErrorMessage = "PageSize must be between 1 and 10.")]

    public int PageSize { get; set; } = 10;

    public bool SortDescending { get; set; } = false;
}