using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class RoomService : IRoomService
{

    private readonly IRoomRepository _roomRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageUploader _imageUploader;
    private readonly IMapper _mapper;

    public RoomService(IRoomRepository roomRepository, IHotelRepository hotelRepository, IImageUploader imageUploader, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _hotelRepository = hotelRepository;
        _imageUploader = imageUploader;
        _mapper = mapper;
    }
    public async Task<(IEnumerable<RoomDto>, PaginationMetaData)> GetAllAsync(RoomQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _roomRepository.GetAllAsync(queryParams);
        var rooms = _mapper.Map<IEnumerable<RoomDto>>(entities);
        return (rooms, paginationMetaData);
    }
    public async Task<RoomDto?> CreateAsync(RoomCreationDto entity, List<IFormFile> files)
    {
        var hotel = await _hotelRepository.GetByIdAsync(entity.HotelId);
        if (hotel is null)
        {
            return null;
        }
        var room = _mapper.Map<Room>(entity);
        room.CreatedAt = DateTime.UtcNow;
        room.UpdatedAt = DateTime.UtcNow;
        hotel.FolderPath = await _imageUploader.UploadImagesAsync(files, ImageEntityType.Rooms);
        var creationResult = await _roomRepository.CreateAsync(room);
        if (creationResult is null)
        {
            return null;
        }
        await _roomRepository.SaveChangesAsync();
        return _mapper.Map<RoomDto>(creationResult);
    }
    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(int hotelId)
    {
        var rooms = await _roomRepository.GetAvailableRoomsAsync(hotelId);
        return _mapper.Map<IEnumerable<RoomDto>>(rooms);
    }

    public async Task<RoomDto?> UpdateAsync(int id, JsonPatchDocument<RoomUpdateDto> patchDocument)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null)
        {
            return null;
        }
        var updatedRoom = _mapper.Map<RoomUpdateDto>(room);

        patchDocument.ApplyTo(updatedRoom);
        _mapper.Map(updatedRoom, room);
        room.UpdatedAt = DateTime.UtcNow;

        await _roomRepository.SaveChangesAsync();
        return _mapper.Map<RoomDto>(room);
    }

    public async Task<RoomDto?> GetByIdAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(room);
    }
    public async Task<RoomDto?> DeleteAsync(int id)
    {
        var deletedRoom = await _roomRepository.DeleteAsync(id);
        if (deletedRoom is null)
        {
            return null;
        }
        return _mapper.Map<RoomDto>(deletedRoom);
    }
    public async Task<List<string>?> GetImagesPathAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room is null)
        {
            return null;
        }
        var images = await _imageUploader.GetImageUrlsAsync(room.FolderPath);
        return images;
    }
}