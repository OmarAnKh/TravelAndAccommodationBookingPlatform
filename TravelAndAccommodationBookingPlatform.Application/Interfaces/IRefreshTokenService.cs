using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken?> GetValidTokenAsync(string token);
    Task CreateTokenAsync(RefreshToken token);
    Task RevokeTokenAsync(string token);
}