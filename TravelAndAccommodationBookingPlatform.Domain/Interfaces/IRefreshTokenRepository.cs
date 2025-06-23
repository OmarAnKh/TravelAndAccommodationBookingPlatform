using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByIdAsync(int id);
    Task<IEnumerable<RefreshToken>> GetByUserIdAsync(int userId);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task RevokeTokenAsync(string token);
}