using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IRoomService : IService<Room, RoomQueryParameters, RoomCreationDto, RoomUpdateDto, RoomDto>
{
    Task<RoomDto?> GetByIdAsync(int id);
    Task<RoomDto?> DeleteAsync(int id);
    Task<RoomDto?> CreateAsync(RoomCreationDto entity);
    Task<RoomDto?> UpdateAsync(int id, JsonPatchDocument<RoomUpdateDto> patchDocument);

}