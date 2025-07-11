using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation, ReservationQueryParameters>
{
    Task<Reservation?> GetByUserAndRoomIdAsync(int userId, int roomId);
    Task<bool> IsDateRangeOverlappingAsync(int userId, int roomId, DateTime startDate, DateTime endDate);
    Task<bool> IsDateRangeOverlappingForRoomAsync(int roomId, DateTime startDate, DateTime endDate);

}