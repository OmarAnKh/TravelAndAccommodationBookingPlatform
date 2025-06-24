using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly IAppDbContext _context;
    public HotelRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Hotel>, PaginationMetaData)> GetAllAsync(HotelQueryParameters queryParams)
    {
        var query = _context.Hotels.AsQueryable();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            query = query.Where(hotel => hotel.Name.Contains(queryParams.SearchTerm) ||
                                         hotel.Description.Contains(queryParams.SearchTerm));
        }
        query = queryParams.SortDescending
            ? query.OrderByDescending(hotel => hotel.Name)
            : query;
        var totalCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalCount, queryParams.Page, queryParams.PageSize);

        var collectionToReturn = await query
            .Skip(queryParams.PageSize * (queryParams.Page - 1))
            .Take(queryParams.PageSize)
            .ToListAsync();

        return (collectionToReturn, paginationMetaData);
    }
    public async Task<Hotel?> GetByIdAsync(int id)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == id);
        return hotel;
    }
    public async Task<Hotel?> CreateAsync(Hotel entity)
    {
        var result = await _context.Hotels.AddAsync(entity);
        return result.Entity;
    }

    public async Task<Hotel?> DeleteAsync(int id)
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