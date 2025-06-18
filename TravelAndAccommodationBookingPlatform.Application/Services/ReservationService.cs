using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    public ReservationService(IReservationRepository reservationRepository, IRoomRepository roomRepository, IMapper mapper)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ReservationDto>, PaginationMetaData)> GetAllAsync(ReservationQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _reservationRepository.GetAllAsync(queryParams);
        var reservations = _mapper.Map<IEnumerable<ReservationDto>>(entities);
        return (reservations, paginationMetaData);
    }
    public async Task<ReservationDto?> CreateAsync(ReservationCreationDto entity, int userId)
    {
        var room = await _roomRepository.GetByIdAsync(entity.RoomId);
        var isDateValid = DateValidation(entity.StartDate, entity.EndDate);
        if (!isDateValid)
        {
            return null;
        }
        if (room is null)
        {
            return null;
        }
        var reservation = _mapper.Map<Reservation>(entity);
        reservation.UserId = userId;
        reservation.BookDate = DateTime.Now;

        var result = await _reservationRepository.CreateAsync(reservation);
        if (result is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(result);
    }
    public async Task<ReservationDto?> UpdateAsync(int userId, int roomId, JsonPatchDocument<ReservationUpdateDto> patchDocument)
    {
        var reservation = await _reservationRepository.GetByUserAndRoomIdAsync(userId, roomId);
        if (reservation is null)
        {
            return null;
        }
        var reservationUpdateDto = _mapper.Map<ReservationUpdateDto>(reservation);
        patchDocument.ApplyTo(reservationUpdateDto);

        _mapper.Map(reservationUpdateDto, reservation);
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(reservation);
    }


    public async Task<ReservationDto?> GetByUserAndRoomIdAsync(int userId, int roomId)
    {
        var reservation = await _reservationRepository.GetByUserAndRoomIdAsync(userId, roomId);
        if (reservation is null)
        {
            return null;
        }
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<ReservationDto?> DeleteByUserAndRoomIdAsync(int userId, int roomId)
    {
        var reservation = await _reservationRepository.DeleteByUserAndRoomIdAsync(userId, roomId);
        if (reservation is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<bool> MarkAsPaidAsync(int userId, int roomId)
    {
        var reservation = await _reservationRepository.GetByUserAndRoomIdAsync(userId, roomId);
        if (reservation is null)
        {
            return false;
        }
        reservation.PaymentStatus = PaymentStatus.Completed;
        reservation.BookingStatus = BookingStatus.Confirmed;
        await _reservationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsFailedAsync(int userId, int roomId)
    {
        var reservation = await _reservationRepository.GetByUserAndRoomIdAsync(userId, roomId);
        if (reservation is null)
        {
            return false;
        }
        reservation.PaymentStatus = PaymentStatus.Failed;
        reservation.BookingStatus = BookingStatus.Cancelled;
        await _reservationRepository.SaveChangesAsync();
        return true;
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