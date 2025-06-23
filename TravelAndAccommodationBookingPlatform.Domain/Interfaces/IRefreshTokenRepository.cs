using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task RevokeTokenAsync(string token);
    Task RevokeAllTokensAsync(int userId);

}