using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface ILocationRepository : IRepository<Location, LocationQueryParameters>
{
    Task<Location?> GetByIdAsync(int id);
    Task<Location?> DeleteAsync(int id);
}