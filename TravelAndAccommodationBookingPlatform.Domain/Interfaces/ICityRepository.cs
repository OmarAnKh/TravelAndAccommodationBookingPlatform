using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface ICityRepository : IRepository<City, CityQueryParameters>
{
    Task<City?> GetById(int id);
    Task<City?> Delete(int id);
}