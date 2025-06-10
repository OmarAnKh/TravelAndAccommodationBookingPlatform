using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly IAppDbContext _context;

    public LocationRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Location>, PaginationMetaData)> GetAll(IQueryParameters parameters)
    {
        var locationQueryParameters = parameters as LocationQueryParameters;
        var pageSize = locationQueryParameters?.PageSize ?? 10;
        var pageNumber = locationQueryParameters?.Page ?? 1;
        int? hotelId = locationQueryParameters?.HotelId;
        float? latitude = locationQueryParameters?.Latitude;
        float? longitude = locationQueryParameters?.Longitude;

        var query = _context.Locations.Where(location =>
            (hotelId.HasValue && location.HotelId == hotelId) ||
            (latitude.HasValue && longitude.HasValue && location.Latitude == latitude && location.Longitude == longitude));

        int totalItemsCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalItemsCount, pageNumber, pageSize);

        var resultCollection = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return (resultCollection, paginationMetaData);
    }

    public async Task<Location?> GetById(int id)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.HotelId == id);
        return location;
    }
    public async Task<Location?> Create(Location entity)
    {
        await _context.Locations.AddAsync(entity);
        return entity;
    }
    public async Task<Location?> UpdateAsync(Location entity)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(c => c.HotelId == entity.HotelId);
        if (location == null)
        {
            return null;
        }
        _context.Entry(location).CurrentValues.SetValues(entity);
        return location;
    }
    public async Task<Location?> Delete(int id)
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