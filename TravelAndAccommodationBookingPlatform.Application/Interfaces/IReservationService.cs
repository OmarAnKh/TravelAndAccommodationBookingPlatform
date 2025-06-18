using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReservationService : IService<Reservation, ReservationQueryParameters, ReservationCreationDto, ReservationUpdateDto, ReservationDto>
{
    Task<ReservationDto?> GetByUserAndRoomIdAsync(int userId, int roomId);
    Task<ReservationDto?> DeleteByUserAndRoomIdAsync(int userId, int roomId);
    Task<ReservationDto?> CreateAsync(ReservationCreationDto entity, int userId);
    Task<ReservationDto?> UpdateAsync(int userId, int roomId, JsonPatchDocument<ReservationUpdateDto> patchDocument);
    Task<bool> MarkAsPaidAsync(int userId, int roomId);
    Task<bool> MarkAsFailedAsync(int userId, int roomId);


}