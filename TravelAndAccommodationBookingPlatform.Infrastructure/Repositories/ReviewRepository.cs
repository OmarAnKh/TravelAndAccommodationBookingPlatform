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
    public async Task<(IEnumerable<Review>, PaginationMetaData)> GetAllAsync(ReviewQueryParameters queryParams)
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
            query = query.Where(r => r.Rate >= queryParams.Rating);
        }

        var totalItemCount = await query.CountAsync();
        var paginationMetaData = new PaginationMetaData(totalItemCount, queryParams.Page, queryParams.PageSize);
        var pagedList = await query.Skip(queryParams.PageSize * (queryParams.Page - 1)).Take(queryParams.PageSize).ToListAsync();
        return (pagedList, paginationMetaData);
    }
    public async Task<Review?> CreateAsync(Review entity)
    {
        var review = await _context.Reviews.AddAsync(entity);
        return review.Entity;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
    }
    public async Task<Review?> DeleteAsync(int id)
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