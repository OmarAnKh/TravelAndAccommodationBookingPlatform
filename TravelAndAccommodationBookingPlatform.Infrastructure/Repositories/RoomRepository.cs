using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{

    private readonly IAppDbContext _context;
    public RoomRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Room>, PaginationMetaData)> GetAllAsync(RoomQueryParameters queryParams)
    {
        var query = _context.Rooms
            .AsQueryable();

        if (queryParams.HotelId.HasValue)
        {
            query = query.Where(r => r.HotelId == queryParams.HotelId.Value);
        }

        if (queryParams.RoomType.HasValue)
        {
            query = query.Where(r => r.RoomType == queryParams.RoomType.Value);
        }
        if (queryParams.MinPrice.HasValue)
        {
            query = query.Where(r => r.Price >= queryParams.MinPrice.Value);
        }
        if (queryParams.MaxPrice.HasValue)
        {
            query = query.Where(r => r.Price <= queryParams.MaxPrice.Value);
        }
        if (queryParams.Adults.HasValue)
        {
            query = query.Where(r => r.Adults >= queryParams.Adults.Value);
        }

        if (queryParams.Children.HasValue)
        {
            query = query.Where(r => r.Children >= queryParams.Children.Value);
        }

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var pagination = new PaginationMetaData(totalCount, queryParams.Page, queryParams.PageSize);

        return (rooms, pagination);
    }

    public async Task<Room?> CreateAsync(Room entity)
    {
        var result = await _context.Rooms.AddAsync(entity);
        return result.Entity;
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(int hotelId)
    {
        var availableRooms = await _context.Rooms
            .Where(room => room.HotelId == hotelId &&
                           room.Availability == Availability.Available)
            .ToListAsync();
        return availableRooms;
    }
    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Room?> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
        if (room == null)
        {
            return null;
        }
        _context.Rooms.Remove(room);
        return room;
    }
}