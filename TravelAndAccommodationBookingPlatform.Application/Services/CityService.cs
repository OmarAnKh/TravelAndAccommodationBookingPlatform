using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;

    public CityService(ICityRepository cityService, IMapper mapper)
    {
        _cityRepository = cityService;
        _mapper = mapper;
    }


    public async Task<(IEnumerable<CityDto>, PaginationMetaData)> GetAll(CityQueryParameters queryParams)
    {
        var (cities, paginationMetaData) = await _cityRepository.GetAll(queryParams);
        var citiesDto = _mapper.Map<IEnumerable<CityDto>>(cities);
        return (citiesDto, paginationMetaData);
    }


    public async Task<CityDto?> Create(CityCreationDto entity)
    {
        var city = _mapper.Map<City>(entity);
        city.CreatedAt = DateTime.UtcNow;
        city.UpdatedAt = DateTime.UtcNow;
        var creationResult = await _cityRepository.Create(city);
        if (creationResult == null)
        {
            return null;
        }
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityDto>(creationResult);
    }


    public async Task<CityDto?> UpdateAsync(CityUpdateDto entity)
    {
        var city = _mapper.Map<City>(entity);
        city.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _cityRepository.UpdateAsync(city);
        if (updateResult == null)
        {
            return null;
        }
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityDto>(updateResult);
    }


    public async Task<CityDto?> GetById(int id)
    {
        var city = await _cityRepository.GetById(id);
        if (city == null)
        {
            return null;
        }
        return _mapper.Map<CityDto>(city);
    }


    public async Task<CityDto?> Delete(int id)
    {
        var deleteResult = await _cityRepository.Delete(id);
        if (deleteResult == null)
        {
            return null;
        }
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityDto>(deleteResult);
    }
}