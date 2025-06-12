using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ICityService : IService<City, CityQueryParameters>
{
    Task<City?> GetById(int id);
    Task<City?> Delete(int id);
}