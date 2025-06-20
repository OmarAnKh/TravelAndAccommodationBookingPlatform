using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ICityService : IService<City, CityQueryParameters, CityCreationDto, CityUpdateDto, CityDto>, IImagesService
{
    Task<CityDto?> CreateAsync(CityCreationDto entity, List<IFormFile> files);

}