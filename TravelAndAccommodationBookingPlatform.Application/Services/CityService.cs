using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IImageUploader _imageUploader;
    private readonly IMapper _mapper;

    public CityService(ICityRepository cityService, IMapper mapper, IImageUploader imageUploader)
    {
        _cityRepository = cityService;
        _mapper = mapper;
        _imageUploader = imageUploader;
    }


    public async Task<(IEnumerable<CityDto>, PaginationMetaData)> GetAllAsync(CityQueryParameters queryParams)
    {
        var (cities, paginationMetaData) = await _cityRepository.GetAllAsync(queryParams);
        var citiesDto = _mapper.Map<IEnumerable<CityDto>>(cities);
        return (citiesDto, paginationMetaData);
    }


    public async Task<CityDto?> CreateAsync(CityCreationDto entity, List<IFormFile> files)
    {
        var imagePath = await _imageUploader.UploadImagesAsync(files, ImageEntityType.Cities);
        var city = _mapper.Map<City>(entity);
        city.CreatedAt = DateTime.UtcNow;
        city.UpdatedAt = DateTime.UtcNow;
        city.FolderPath = imagePath;
        var creationResult = await _cityRepository.CreateAsync(city);
        if (creationResult == null)
        {
            return null;
        }
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityDto>(creationResult);
    }


    public async Task<CityDto?> UpdateAsync(int id, JsonPatchDocument<CityUpdateDto> patchDocument)
    {
        var city = await _cityRepository.GetByIdAsync(id);

        if (city == null)
        {
            return null;
        }

        var cityToPatch = _mapper.Map<CityUpdateDto>(city);
        patchDocument.ApplyTo(cityToPatch);

        _mapper.Map(cityToPatch, city);
        city.UpdatedAt = DateTime.UtcNow;

        await _cityRepository.SaveChangesAsync();

        return _mapper.Map<CityDto>(city);
    }


    public async Task<CityDto?> GetByIdAsync(int id)
    {
        var city = await _cityRepository.GetByIdAsync(id);
        if (city == null)
        {
            return null;
        }
        return _mapper.Map<CityDto>(city);
    }


    public async Task<CityDto?> DeleteAsync(int id)
    {
        var deleteResult = await _cityRepository.DeleteAsync(id);
        if (deleteResult == null)
        {
            return null;
        }
        await _cityRepository.SaveChangesAsync();
        return _mapper.Map<CityDto>(deleteResult);
    }

    public async Task<List<string>?> GetImagesPathAsync(int cityId)
    {
        var city = await _cityRepository.GetByIdAsync(cityId);
        if (city is null)
        {
            return null;
        }
        var images = await _imageUploader.GetImageUrlsAsync(city.FolderPath);
        return images;
    }
}