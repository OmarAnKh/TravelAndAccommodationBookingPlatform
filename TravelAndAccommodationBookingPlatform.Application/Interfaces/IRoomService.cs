using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IRoomService : IService<Room, RoomQueryParameters, RoomCreationDto, RoomUpdateDto, RoomDto>
{
    Task<Room?> GetById(int id);
    Task<Room?> Delete(int id);
}