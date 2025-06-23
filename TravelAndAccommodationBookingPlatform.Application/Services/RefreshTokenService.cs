using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _tokenRepository;

    public RefreshTokenService(IRefreshTokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token)
    {
        var refreshToken = await _tokenRepository.GetByTokenAsync(token);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt <= DateTime.UtcNow)
            return null;

        return refreshToken;
    }

    public async Task CreateTokenAsync(RefreshToken token)
    {
        await _tokenRepository.AddAsync(token);
    }

    public async Task RevokeTokenAsync(string token)
    {
        await _tokenRepository.RevokeTokenAsync(token);
    }
    public async Task RevokeAllTokensForUserAsync(int userId)
    {
        await _tokenRepository.RevokeAllTokensAsync(userId);
    }
}