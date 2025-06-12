using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly IAppDbContext _context;

    public ReviewRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<Review>, PaginationMetaData)> GetAll(ReviewQueryParameters queryParams)
    {
        var query = _context.Reviews as IQueryable<Review>;
        if (queryParams.UserId != null)
        {
            query = query.Where(r => r.UserId == queryParams.UserId);
        }
        if (queryParams.HotelId != null)
        {
            query = query.Where(r => r.HotelId == queryParams.HotelId);
        }
        if (queryParams.Rating != null)
        {
            query = query.Where(r => r.Rate == queryParams.Rating);
        }

        var totalItemCount = await _context.Reviews.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalItemCount, queryParams.Page, queryParams.PageSize);
        var pagedList = await query.Skip(queryParams.PageSize * (queryParams.Page - 1)).Take(queryParams.PageSize).ToListAsync();
        return (pagedList, paginationMetaData);
    }
    public async Task<Review?> Create(Review entity)
    {
        var review = await _context.Reviews.AddAsync(entity);
        return review.Entity;
    }
    public async Task<Review?> UpdateAsync(Review entity)
    {
        var entry = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == entity.ReviewId);
        if (entry == null)
        {
            return null;
        }
        _context.Entry(entry).CurrentValues.SetValues(entity);
        return entry;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task<Review?> GetById(int id)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
    }
    public async Task<Review?> Delete(int id)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
        if (review == null)
        {
            return null;
        }
        _context.Reviews.Remove(review);
        return review;
    }
}