using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly SqlServerDbContext _context;
    public HotelRepository(SqlServerDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Hotel>, PaginationMetaData)> GetAll(IQueryParameters parameters)
    {
        var hotelParameters = parameters as HotelQueryParameters;
        var pageNumber = hotelParameters?.Page ?? 1;
        var pageSize = hotelParameters?.PageSize ?? 10;
        var query = _context.Hotels as IQueryable<Hotel>;
        if (!string.IsNullOrEmpty(hotelParameters?.SearchTerm))
        {
            query = query.Where(hotel => hotel.Name.Contains(hotelParameters.SearchTerm)
                                         || hotel.Description.Contains(hotelParameters.SearchTerm)
            );
        }
        var totalCount = await query.CountAsync();
        PaginationMetaData paginationMetaData = new PaginationMetaData(totalCount, pageNumber, pageSize);
        var collectionToReturn = await query
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();
        return (collectionToReturn, paginationMetaData);
    }
    public async Task<Hotel?> GetById(int id)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
        return hotel;
    }
    public async Task<Hotel?> Create(Hotel entity)
    {
        await _context.Hotels.AddAsync(entity);
        return entity;
    }
    public Task<Hotel?> UpdateAsync(Hotel entity)
    {
        var entityAfterUpdate = _context.Hotels.Update(entity).Entity;

        // return entityAfterUpdate;
        return Task.FromResult(entityAfterUpdate);
    }

    public async Task<Hotel?> Delete(int id)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
        if (hotel == null)
        {
            return null;
        }
        _context.Hotels.Remove(hotel);
        return hotel;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}