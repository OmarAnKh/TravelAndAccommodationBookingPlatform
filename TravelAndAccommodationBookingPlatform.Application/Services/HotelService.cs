using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public HotelService(IHotelRepository hotelRepository, IMapper mapper)
    {
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }
    public async Task<(IEnumerable<HotelDto>, PaginationMetaData)> GetAll(HotelQueryParameters queryParams)
    {
        var (entities, metaData) = await _hotelRepository.GetAll(queryParams);
        var hotels = _mapper.Map<IEnumerable<HotelDto>>(entities);
        return (hotels, metaData);
    }
    public async Task<HotelDto?> Create(HotelCreationDto entity)
    {
        var hotel = _mapper.Map<Hotel>(entity);
        var createResult = await _hotelRepository.Create(hotel);
        if (createResult is null)
        {
            return null;
        }
        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<HotelDto>(createResult);

    }
    public async Task<HotelDto?> UpdateAsync(HotelUpdateDto entity)
    {
        var hotel = _mapper.Map<Hotel>(entity);
        var updateResult = await _hotelRepository.UpdateAsync(hotel);
        if (updateResult is null)
        {
            return null;
        }
        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<HotelDto>(updateResult);
    }
    public async Task<Hotel?> GetById(int id)
    {
        var hotel = await _hotelRepository.GetById(id);
        if (hotel is null)
        {
            return null;
        }
        return _mapper.Map<Hotel>(hotel);
    }
    public async Task<Hotel?> Delete(int id)
    {
        var deleteResult = await _hotelRepository.Delete(id);
        if (deleteResult is null)
        {
            return null;
        }
        await _hotelRepository.SaveChangesAsync();
        return _mapper.Map<Hotel>(deleteResult);
    }
}