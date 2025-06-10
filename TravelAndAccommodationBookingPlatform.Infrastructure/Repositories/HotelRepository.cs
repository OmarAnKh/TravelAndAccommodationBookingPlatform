using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly IAppDbContext _context;
    public HotelRepository(IAppDbContext context)
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
    public async Task<Hotel?> UpdateAsync(Hotel entity)
    {

        var hotel = await _context.Hotels.FirstOrDefaultAsync(c => c.Id == entity.Id);
        if (hotel == null)
        {
            return null;
        }
        _context.Entry(hotel).CurrentValues.SetValues(entity);
        return hotel;
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