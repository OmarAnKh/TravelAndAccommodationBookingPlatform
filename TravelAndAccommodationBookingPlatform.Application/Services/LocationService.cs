using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public LocationService(ILocationRepository locationRepository, IHotelRepository hotelRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }
    public async Task<(IEnumerable<LocationDto>, PaginationMetaData)> GetAllAsync(LocationQueryParameters queryParams)
    {
        var (entities, metaData) = await _locationRepository.GetAllAsync(queryParams);
        var locations = _mapper.Map<IEnumerable<LocationDto>>(entities);
        return (locations, metaData);
    }
    public async Task<LocationDto?> UpdateAsync(int id, JsonPatchDocument<LocationUpdateDto> patchDocument)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location is null)
        {
            return null;
        }
        var locationToPatch = _mapper.Map<LocationUpdateDto>(location);

        patchDocument.ApplyTo(locationToPatch);

        _mapper.Map(locationToPatch, location);

        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(location);
    }

    public async Task<LocationDto?> CreateAsync(LocationCreationDto entity)
    {
        var location = _mapper.Map<Location>(entity);
        var hotel = await _hotelRepository.GetByIdAsync(location.HotelId);
        if (hotel is null)
        {
            return null;
        }
        var creationResult = await _locationRepository.CreateAsync(location);
        if (creationResult is null)
        {
            return null;
        }
        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(creationResult);

    }

    public async Task<LocationDto?> GetByIdAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location is null)
        {
            return null;
        }
        return _mapper.Map<LocationDto>(location);
    }

    public async Task<LocationDto?> DeleteAsync(int id)
    {
        var deleteResult = await _locationRepository.DeleteAsync(id);
        if (deleteResult is null)
        {
            return null;
        }
        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(deleteResult);
    }
}