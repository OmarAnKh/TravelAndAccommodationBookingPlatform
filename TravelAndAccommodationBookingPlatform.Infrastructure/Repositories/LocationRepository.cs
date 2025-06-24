using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly IAppDbContext _context;

    public LocationRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Location>, PaginationMetaData)> GetAllAsync(LocationQueryParameters queryParams)
    {
        var query = _context.Locations.AsQueryable();

        if (queryParams.HotelId > 0)
        {
            query = query.Where(location => location.HotelId == queryParams.HotelId);
        }

        bool hasLat = Math.Abs(queryParams.Latitude) > 0.00001;
        bool hasLng = Math.Abs(queryParams.Longitude) > 0.00001;
        if (hasLat)
        {
            query = query.Where(location =>
                Math.Abs(location.Latitude - queryParams.Latitude) < 0.0001);
        }
        if (hasLng)
        {
            query = query.Where(location =>
                Math.Abs(location.Longitude - queryParams.Longitude) < 0.0001);
        }

        int totalItemsCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalItemsCount, queryParams.Page, queryParams.PageSize);

        var resultCollection = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (resultCollection, paginationMetaData);
    }


    public async Task<Location?> GetByIdAsync(int id)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.HotelId == id);
        return location;
    }
    public async Task<Location?> CreateAsync(Location entity)
    {
        var result = await _context.Locations.AddAsync(entity);
        return result.Entity;
    }

    public async Task<Location?> DeleteAsync(int id)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.HotelId == id);
        if (location == null)
        {
            return null;
        }
        _context.Locations.Remove(location);
        return location;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}