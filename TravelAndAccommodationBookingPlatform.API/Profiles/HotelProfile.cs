using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.API.Profiles;

public class HotelProfile : Profile
{
    public HotelProfile()
    {
        CreateMap<Hotel, HotelDto>();
        CreateMap<HotelCreationDto, Hotel>();
        CreateMap<HotelUpdateDto, Hotel>();
        CreateMap<Hotel, HotelUpdateDto>();
        CreateMap<HotelUpdateDto, HotelDto>();
    }
}