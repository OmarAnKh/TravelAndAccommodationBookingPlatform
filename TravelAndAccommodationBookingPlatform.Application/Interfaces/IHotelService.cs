using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IHotelService : IService<Hotel, HotelQueryParameters, HotelCreationDto, HotelUpdateDto, HotelDto>
{
    Task<HotelDto?> GetByIdAsync(int id);
    Task<HotelDto?> DeleteAsync(int id);
    Task<HotelDto?> CreateAsync(HotelCreationDto entity, List<IFormFile> thumbnails);

    Task<HotelDto?> UpdateAsync(int id, JsonPatchDocument<HotelUpdateDto> patchDocument);

}