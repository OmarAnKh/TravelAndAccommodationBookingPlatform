using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;

public class ReservationQueryParameters : IQueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public BookingStatus? BookingStatus { get; set; }


}