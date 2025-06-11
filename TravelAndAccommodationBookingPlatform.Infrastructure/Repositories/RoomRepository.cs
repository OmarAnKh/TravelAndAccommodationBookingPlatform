using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{

    private readonly IAppDbContext _context;
    public RoomRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Room>, PaginationMetaData)> GetAll(RoomQueryParameters queryParams)
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

    public async Task<Room?> Create(Room entity)
    {
        var result = await _context.Rooms.AddAsync(entity);
        return result.Entity;
    }

    public async Task<Room?> UpdateAsync(Room entity)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == entity.Id);
        if (room == null)
        {
            return null;
        }
        _context.Entry(room).CurrentValues.SetValues(entity);
        return room;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task<Room?> GetById(int id)
    {
        return await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Room?> Delete(int id)
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