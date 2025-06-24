namespace TravelAndAccommodationBookingPlatform.API.Common.Interfaces;

public interface IJwtProvider
{
    string SecretKey { get; }
    string Issuer { get; }
    string Audience { get; }
}