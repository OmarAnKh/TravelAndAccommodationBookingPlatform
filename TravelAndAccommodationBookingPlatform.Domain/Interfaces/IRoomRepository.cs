using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRoomRepository : IRepository<Room, RoomQueryParameters>
{
    Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId);

}