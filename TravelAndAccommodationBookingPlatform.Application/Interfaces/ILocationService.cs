using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ILocationService : IService<Location, LocationQueryParameters, LocationCreationDto, LocationUpdateDto, LocationDto>
{
    Task<Location?> GetById(int id);
    Task<Location?> Delete(int id);
}