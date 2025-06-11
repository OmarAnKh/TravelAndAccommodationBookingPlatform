using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

public class UserQueryParameters : IQueryParameters
{

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public UserRole? Role { get; set; }
    public DateTime? MinBirthDate { get; set; }
    public DateTime? MaxBirthDate { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
}