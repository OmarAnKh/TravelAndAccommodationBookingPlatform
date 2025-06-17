using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface ICityRepository : IRepository<City, CityQueryParameters>
{
    Task<City?> GetByIdAsync(int id);
    Task<City?> DeleteAsync(int id);
}