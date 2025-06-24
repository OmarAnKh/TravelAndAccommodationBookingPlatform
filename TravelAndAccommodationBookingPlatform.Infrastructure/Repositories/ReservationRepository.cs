using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly IAppDbContext _context;

    public ReservationRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Reservation>, PaginationMetaData)> GetAllAsync(ReservationQueryParameters queryParams)
    {
        var query = _context.Reservations.AsQueryable();

        if (queryParams.StartDate.HasValue)
        {
            query = query.Where(r => r.StartDate >= queryParams.StartDate.Value);
        }

        if (queryParams.EndDate.HasValue)
        {
            query = query.Where(r => r.EndDate <= queryParams.EndDate.Value);
        }

        if (queryParams.PaymentStatus.HasValue)
        {
            query = query.Where(r => r.PaymentStatus == queryParams.PaymentStatus.Value);
        }

        if (queryParams.BookingStatus.HasValue)
        {
            query = query.Where(r => r.BookingStatus == queryParams.BookingStatus.Value);
        }

        int totalCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalCount, queryParams.Page, queryParams.PageSize);

        var collectionToReturn = await query
            .Skip(queryParams.PageSize * (queryParams.Page - 1))
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetaData);
    }
    public async Task<Reservation?> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        return reservation;
    }
    public async Task<Reservation?> DeleteAsync(int id)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (reservation == null)
        {
            return null;
        }
        _context.Reservations.Remove(reservation);
        return reservation;
    }

    public async Task<Reservation?> GetByUserAndRoomIdAsync(int userId, int roomId)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.UserId == userId && r.RoomId == roomId);
        return reservation;
    }


    public async Task<Reservation?> CreateAsync(Reservation entity)
    {
        var result = await _context.Reservations.AddAsync(entity);
        return result.Entity;
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> IsDateRangeOverlappingAsync(int userId, int roomId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .AnyAsync(r =>
                r.UserId == userId &&
                r.RoomId == roomId &&
                (r.PaymentStatus == PaymentStatus.Completed || r.BookingStatus == BookingStatus.Confirmed) &&
                (
                    (startDate >= r.StartDate && startDate < r.EndDate) ||
                    (endDate > r.StartDate && endDate <= r.EndDate) ||
                    (startDate <= r.StartDate && endDate >= r.EndDate)
                )
            );
    }

    public async Task<bool> IsDateRangeOverlappingForRoomAsync(int roomId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .AnyAsync(r =>
                r.RoomId == roomId &&
                (r.PaymentStatus == PaymentStatus.Completed || r.BookingStatus == BookingStatus.Confirmed) &&
                (
                    (startDate >= r.StartDate && startDate < r.EndDate) ||
                    (endDate > r.StartDate && endDate <= r.EndDate) ||
                    (startDate <= r.StartDate && endDate >= r.EndDate)
                )
            );
    }

}