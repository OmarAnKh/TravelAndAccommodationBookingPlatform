using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.API.Profiles;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<CityCreationDto, City>();
        CreateMap<City, CityDto>();
        CreateMap<CityUpdateDto, City>();
    }
}