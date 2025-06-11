using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface ILocationRepository : IRepository<Location, LocationQueryParameters>
{
    Task<Location?> GetById(int id);
    Task<Location?> Delete(int id);
}