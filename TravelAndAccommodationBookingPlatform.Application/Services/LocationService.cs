using AutoMapper;
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
    public async Task<(IEnumerable<LocationDto>, PaginationMetaData)> GetAll(LocationQueryParameters queryParams)
    {
        var (entities, metaData) = await _locationRepository.GetAll(queryParams);
        var locations = _mapper.Map<IEnumerable<LocationDto>>(entities);
        return (locations, metaData);
    }

    public async Task<LocationDto?> Create(LocationCreationDto entity)
    {
        var location = _mapper.Map<Location>(entity);
        var hotel = await _hotelRepository.GetById(location.HotelId);
        if (hotel is null)
        {
            return null;
        }
        var creationResult = await _locationRepository.Create(location);
        if (creationResult is null)
        {
            return null;
        }
        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(creationResult);

    }
    public async Task<LocationDto?> UpdateAsync(LocationUpdateDto entity)
    {
        var location = _mapper.Map<Location>(entity);
        var updateResult = await _locationRepository.UpdateAsync(location);
        if (updateResult is null)
        {
            return null;
        }
        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(updateResult);
    }
    public async Task<LocationDto?> GetById(int id)
    {
        var location = await _locationRepository.GetById(id);
        if (location is null)
        {
            return null;
        }
        return _mapper.Map<LocationDto>(location);
    }

    public async Task<LocationDto?> Delete(int id)
    {
        var deleteResult = await _locationRepository.Delete(id);
        if (deleteResult is null)
        {
            return null;
        }
        await _locationRepository.SaveChangesAsync();
        return _mapper.Map<LocationDto>(deleteResult);
    }
}