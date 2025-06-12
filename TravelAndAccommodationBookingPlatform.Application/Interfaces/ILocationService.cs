using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ILocationService : IService<Location, LocationQueryParameters>
{
    Task<Location?> GetById(int id);
    Task<Location?> Delete(int id);
}