using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRoomRepository : IRepository<Room, RoomQueryParameters>
{
    Task<Room?> GetById(int id);
    Task<Room?> Delete(int id);
}