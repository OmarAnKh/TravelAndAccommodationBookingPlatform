using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReservationService : IService<Reservation, ReservationQueryParameters, ReservationCreationDto, ReservationUpdateDto, ReservationDto>
{
    Task<ReservationDto?> GetByUserAndRoomIdAsync(int userId, int roomId);
    Task<ReservationDto?> CreateAsync(ReservationCreationDto entity, int userId);
    Task<bool> MarkAsPaidAsync(int reservationId);
    Task<bool> MarkAsFailedAsync(int reservationId);
    Task<bool> MarkAsCancelledAsync(int reservationId);
}