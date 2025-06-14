using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class RoomService : IRoomService
{

    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IHotelRepository hotelRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }
    public async Task<(IEnumerable<RoomDto>, PaginationMetaData)> GetAll(RoomQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _roomRepository.GetAll(queryParams);
        var rooms = _mapper.Map<IEnumerable<RoomDto>>(entities);
        return (rooms, paginationMetaData);
    }
    public async Task<RoomDto?> Create(RoomCreationDto entity)
    {
        var hotel = await _hotelRepository.GetById(entity.HotelId);
        if (hotel is null)
        {
            return null;
        }
        var room = _mapper.Map<Room>(entity);
        room.CreatedAt = DateTime.UtcNow;
        room.UpdatedAt = DateTime.UtcNow;
        var creationResult = await _roomRepository.Create(room);
        if (creationResult is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(creationResult);
    }
    public async Task<RoomDto?> UpdateAsync(RoomUpdateDto entity)
    {
        var room = _mapper.Map<Room>(entity);
        room.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _roomRepository.UpdateAsync(room);
        if (updateResult is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(updateResult);
    }
    public async Task<RoomDto?> GetById(int id)
    {
        var room = await _roomRepository.GetById(id);
        if (room is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(room);
    }
    public async Task<RoomDto?> Delete(int id)
    {
        var deletedRoom = await _roomRepository.Delete(id);
        if (deletedRoom is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(deletedRoom);
    }
}