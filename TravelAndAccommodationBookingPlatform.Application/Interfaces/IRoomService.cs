using Microsoft.AspNetCore.Http;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IRoomService : IService<Room, RoomQueryParameters, RoomCreationDto, RoomUpdateDto, RoomDto>, IImagesService
{
    Task<RoomDto?> CreateAsync(RoomCreationDto entity, List<IFormFile> files);
    Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(int hotelId);

}