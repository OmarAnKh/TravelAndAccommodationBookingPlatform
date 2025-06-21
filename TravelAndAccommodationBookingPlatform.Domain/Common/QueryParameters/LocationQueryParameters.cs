using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class LocationQueryParameters : IQueryParameters
{

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 10, ErrorMessage = "PageSize must be between 1 and 10.")]
    public int PageSize { get; set; } = 10;

    public int HotelId { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
}