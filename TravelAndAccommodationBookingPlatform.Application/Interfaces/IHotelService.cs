using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IHotelService : IService<Hotel, HotelQueryParameters, HotelCreationDto, HotelUpdateDto, HotelDto>
{
    Task<HotelDto?> GetById(int id);
    Task<HotelDto?> Delete(int id);
}