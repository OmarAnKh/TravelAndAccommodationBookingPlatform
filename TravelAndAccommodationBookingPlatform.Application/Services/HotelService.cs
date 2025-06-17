using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IImageUploader _imageUploader;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepository, ICityRepository cityRepository, IImageUploader imageUploader, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _cityRepository = cityRepository;
        _imageUploader = imageUploader;
        _mapper = mapper;

    }
    public async Task<(IEnumerable<HotelDto>, PaginationMetaData)> GetAllAsync(HotelQueryParameters queryParams)
    {
        var (entities, metaData) = await _hotelRepository.GetAllAsync(queryParams);
        var hotels = _mapper.Map<IEnumerable<HotelDto>>(entities);
        return (hotels, metaData);
    }

    public async Task<HotelDto?> CreateAsync(HotelCreationDto entity, List<IFormFile> thumbnailsse)
    {
        var city = await _cityRepository.GetByIdAsync(entity.CityId);
        if (city == null)
        {
            return null;
        }
        var imagesPath = await _imageUploader.UploadImagesAsync(thumbnailsse, ImageEntityType.Hotels);
        var hotel = _mapper.Map<Hotel>(entity);
        hotel.CreatedAt = DateTime.UtcNow;
        hotel.UpdatedAt = DateTime.UtcNow;
        hotel.Thumbnail = imagesPath;

        var createResult = await _hotelRepository.CreateAsync(hotel);
        if (createResult is null)
        {
            return null;
        }

        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<HotelDto>(createResult);
    }

    public async Task<HotelDto?> UpdateAsync(int id, JsonPatchDocument<HotelUpdateDto> patchDocument)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel is null)
        {
            return null;
        }

        var hotelToPatch = _mapper.Map<HotelUpdateDto>(hotel);

        patchDocument.ApplyTo(hotelToPatch);

        _mapper.Map(hotelToPatch, hotel);
        hotel.UpdatedAt = DateTime.UtcNow;

        await _hotelRepository.SaveChangesAsync();

        return _mapper.Map<HotelDto>(hotel);
    }


    public async Task<HotelDto?> GetByIdAsync(int id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel is null)
        {
            return null;
        }
        return _mapper.Map<HotelDto>(hotel);
    }
    public async Task<HotelDto?> DeleteAsync(int id)
    {
        var deleteResult = await _hotelRepository.DeleteAsync(id);
        if (deleteResult is null)
        {
            return null;
        }
        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<HotelDto>(deleteResult);
    }
}