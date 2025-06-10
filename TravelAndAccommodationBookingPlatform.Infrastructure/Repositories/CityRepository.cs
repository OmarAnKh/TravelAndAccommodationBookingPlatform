using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly IAppDbContext _context;

    public CityRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<City>, PaginationMetaData)> GetAll(IQueryParameters parameters)
    {
        var cityParams = parameters as CityQueryParameters;
        var pageNumber = cityParams?.Page ?? 1;
        var pageSize = cityParams?.PageSize ?? 10;
        var query = _context.Cities as IQueryable<City>;

        if (!string.IsNullOrEmpty(cityParams?.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(cityParams.SearchTerm) || c.Country.Contains(cityParams.SearchTerm));
        }
        int totalCount = await query.CountAsync();

        PaginationMetaData paginationMetaData = new PaginationMetaData(totalCount, pageNumber, pageSize);
        var collectionToReturn = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
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
        await _context.Cities.AddAsync(entity);
        return entity;
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