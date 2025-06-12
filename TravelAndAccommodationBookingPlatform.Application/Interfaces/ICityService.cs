using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ICityService : IService<City, CityQueryParameters, CityCreationDto, CityUpdateDto, CityDto>
{
    Task<CityDto?> GetById(int id);
    Task<CityDto?> Delete(int id);
}

//
// public interface ICityService
// {
//     Task<(IEnumerable<CityDto>, PaginationMetaData)> GetAll(CityQueryParameters queryParams);
//     Task<CityDto?> Create(CityCreationDto entity);
//     Task<CityDto?> UpdateAsync(CityUpdateDto entity);
//     Task<CityDto?> GetById(int id);
//     Task<CityDto?> Delete(int id);
// }