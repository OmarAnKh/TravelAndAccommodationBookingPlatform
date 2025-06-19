using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface ILocationService : IService<Location, LocationQueryParameters, LocationCreationDto, LocationUpdateDto, LocationDto>
{
    Task<LocationDto?> CreateAsync(LocationCreationDto entity);


}