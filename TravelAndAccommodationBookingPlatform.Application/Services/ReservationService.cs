using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public ReservationService(IReservationRepository reservationRepository, IRoomRepository roomRepository, IUserRepository userRepository, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ReservationDto>, PaginationMetaData)> GetAll(ReservationQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _reservationRepository.GetAll(queryParams);
        var reservations = _mapper.Map<IEnumerable<ReservationDto>>(entities);
        return (reservations, paginationMetaData);
    }
    public async Task<ReservationDto?> Create(ReservationCreationDto entity)
    {
        var user = await _userRepository.GetById(entity.UserId);
        var room = await _roomRepository.GetById(entity.RoomId);
        var isDateValid = DateValidation(entity.StartDate, entity.EndDate);
        if (!isDateValid)
        {
            return null;
        }
        if (user is null)
        {
            return null;
        }
        if (room is null)
        {
            return null;
        }
        var reservation = _mapper.Map<Reservation>(entity);
        var result = await _reservationRepository.Create(reservation);
        if (result is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(result);
    }
    public async Task<ReservationDto?> UpdateAsync(ReservationUpdateDto entity)
    {
        var reservation = _mapper.Map<Reservation>(entity);
        var result = await _reservationRepository.UpdateAsync(reservation);
        if (result is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(result);
    }

    public async Task<ReservationDto?> GetByUserAndRoomId(int userId, int roomId)
    {
        var reservation = await _reservationRepository.GetByUserAndRoomId(userId, roomId);
        if (reservation is null)
        {
            return null;
        }
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto?> DeleteByUserAndRoomId(int userId, int roomId)
    {
        var reservation = await _reservationRepository.DeleteByUserAndRoomId(userId, roomId);
        if (reservation is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(reservation);
    }

    private bool DateValidation(DateTime startDate, DateTime endDate)
    {
        if (startDate < DateTime.Today)
        {
            return false;
        }
        if (endDate < startDate)
        {
            return false;
        }
        return true;
    }
}