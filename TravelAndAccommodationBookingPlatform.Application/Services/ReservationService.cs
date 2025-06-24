using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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
        if (room is null)
        {
            return null;
        }

        if (!DateValidation(entity.StartDate, entity.EndDate))
        {
            return null;
        }

        var isOverlapping = await _reservationRepository.IsDateRangeOverlappingAsync(userId, entity.RoomId, entity.StartDate, entity.EndDate);
        if (isOverlapping)
        {
            return null;
        }
        var isRoomBooked = await _reservationRepository.IsDateRangeOverlappingForRoomAsync(entity.RoomId, entity.StartDate, entity.EndDate);
        if (isRoomBooked)
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

    public async Task<ReservationDto?> UpdateAsync(int reservationId, JsonPatchDocument<ReservationUpdateDto> patchDocument)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
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

    public async Task<ReservationDto?> GetByIdAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id);
        if (reservation is null)
        {
            return null;
        }
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

    public async Task<ReservationDto?> DeleteAsync(int reservationId)
    {
        var reservation = await _reservationRepository.DeleteAsync(reservationId);
        if (reservation is null)
        {
            return null;
        }
        await _reservationRepository.SaveChangesAsync();
        return _mapper.Map<ReservationDto>(reservation);
    }

    public async Task<bool> MarkAsPaidAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return false;
        }
        reservation.PaymentStatus = PaymentStatus.Completed;
        reservation.BookingStatus = BookingStatus.Confirmed;
        await _reservationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsFailedAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return false;
        }
        reservation.PaymentStatus = PaymentStatus.Failed;
        reservation.BookingStatus = BookingStatus.Cancelled;
        await _reservationRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsCancelledAsync(int reservationId)
    {
        var reservation = await _reservationRepository.GetByIdAsync(reservationId);
        if (reservation is null)
        {
            return false;
        }
        reservation.PaymentStatus = PaymentStatus.Canceled;
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