using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation, ReservationQueryParameters>
{
    Task<Reservation?> GetByUserAndRoomIdAsync(int userId, int roomId);
    Task<Reservation?> DeleteByUserAndRoomIdAsync(int userId, int roomId);
}