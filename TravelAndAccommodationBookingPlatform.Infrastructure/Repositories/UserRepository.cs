using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{

    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<(IEnumerable<User>, PaginationMetaData)> GetAllAsync(UserQueryParameters queryParams)
    {
        var query = ApplyUserFilters(_context.Users.AsQueryable(), queryParams);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((queryParams.Page - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToListAsync();

        var pagination = new PaginationMetaData(totalCount, queryParams.Page, queryParams.PageSize);

        return (users, pagination);
    }

    public async Task<User?> CreateAsync(User entity)
    {
        var result = await _context.Users.AddAsync(entity);
        return result.Entity;
    }


    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User?> DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return null;
        }
        _context.Users.Remove(user);
        return user;
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    }
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> UpdateUserRoleAsync(int userId, UserRole newRole)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        user.Role = newRole;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }
    private static IQueryable<User> ApplyUserFilters(IQueryable<User> query, UserQueryParameters queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.Username))
            query = query.Where(u => u.Username.Contains(queryParams.Username));

        if (!string.IsNullOrWhiteSpace(queryParams.Email))
            query = query.Where(u => u.Email.Contains(queryParams.Email));

        if (queryParams.Role.HasValue)
            query = query.Where(u => u.Role == queryParams.Role.Value);

        if (queryParams.MinBirthDate.HasValue)
            query = query.Where(u => u.BirthDate >= queryParams.MinBirthDate.Value);

        if (queryParams.MaxBirthDate.HasValue)
            query = query.Where(u => u.BirthDate <= queryParams.MaxBirthDate.Value);

        if (queryParams.CreatedAfter.HasValue)
            query = query.Where(u => u.CreatedAt >= queryParams.CreatedAfter.Value);

        if (queryParams.CreatedBefore.HasValue)
            query = query.Where(u => u.CreatedAt <= queryParams.CreatedBefore.Value);

        return query;
    }
}