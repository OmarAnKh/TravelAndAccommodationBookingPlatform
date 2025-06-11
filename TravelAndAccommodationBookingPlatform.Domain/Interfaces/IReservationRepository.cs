using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IReservationRepository : IRepository<Reservation, ReservationQueryParameters>
{
    Task<Reservation?> GetByUserAndRoomId(int userId, int roomId);
    Task<Reservation?> DeleteByUserAndRoomId(int userId, int roomId);
}