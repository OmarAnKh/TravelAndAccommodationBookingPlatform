using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly SqlServerDbContext _context;

    public RefreshTokenRepository(SqlServerDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<RefreshToken?> GetByIdAsync(int id)
    {
        return await _context.RefreshTokens.FindAsync(id);
    }

    public async Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId)
    {
        return await _context.RefreshTokens
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
        }
    }
}