using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReservationService : IService<Reservation, ReservationQueryParameters, ReservationCreationDto, ReservationUpdateDto, ReservationDto>
{
    Task<ReservationDto?> GetByUserAndRoomId(int userId, int roomId);
    Task<ReservationDto?> DeleteByUserAndRoomId(int userId, int roomId);
}