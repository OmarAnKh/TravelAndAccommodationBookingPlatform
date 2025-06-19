using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IRoomService : IService<Room, RoomQueryParameters, RoomCreationDto, RoomUpdateDto, RoomDto>
{
    Task<RoomDto?> CreateAsync(RoomCreationDto entity);

}