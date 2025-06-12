using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly IAppDbContext _context;

    public CityRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<City>, PaginationMetaData)> GetAll(CityQueryParameters queryParams)
    {
        var query = _context.Cities.AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(queryParams.SearchTerm) ||
                                     c.Country.Contains(queryParams.SearchTerm));
        }

        int totalCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalCount, queryParams.Page, queryParams.PageSize);

        var collectionToReturn = await query
            .Skip(queryParams.PageSize * (queryParams.Page - 1))
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetaData);
    }
    public async Task<City?> GetById(int id)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
        return city;
    }
    public async Task<City?> Create(City entity)
    {
        var result = await _context.Cities.AddAsync(entity);
        return result.Entity;
    }
    public async Task<City?> UpdateAsync(City entity)
    {

        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == entity.Id);
        if (city == null)
        {
            return null;
        }

        _context.Entry(city).CurrentValues.SetValues(entity);
        return city;
    }
    public async Task<City?> Delete(int id)
    {
        var city = await _context.Cities.FirstOrDefaultAsync(c => c.Id == id);
        if (city == null)
        {
            return null;
        }
        _context.Cities.Remove(city);
        return city;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}