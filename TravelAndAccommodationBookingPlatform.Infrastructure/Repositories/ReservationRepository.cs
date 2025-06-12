using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
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
    public async Task<(IEnumerable<Reservation>, PaginationMetaData)> GetAll(ReservationQueryParameters queryParams)
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

    public async Task<Reservation?> GetByUserAndRoomId(int userId, int roomId)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.UserId == userId && r.RoomId == roomId);
        return reservation;
    }


    public async Task<Reservation?> Create(Reservation entity)
    {
        var result = await _context.Reservations.AddAsync(entity);
        return result.Entity;
    }


    public async Task<Reservation?> UpdateAsync(Reservation entity)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.UserId == entity.UserId && r.RoomId == entity.RoomId);
        if (reservation == null)
        {
            return null;
        }
        _context.Entry(reservation).CurrentValues.SetValues(entity);
        return reservation;
    }
    public async Task<Reservation?> DeleteByUserAndRoomId(int userId, int roomId)
    {
        var reservation = await _context.Reservations.FirstOrDefaultAsync(r => r.UserId == userId && r.RoomId == roomId);
        if (reservation == null)
        {
            return null;
        }
        _context.Reservations.Remove(reservation);
        return reservation;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}