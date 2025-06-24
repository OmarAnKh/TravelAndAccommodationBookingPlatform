using TravelAndAccommodationBookingPlatform.API.Common.Interfaces;

namespace TravelAndAccommodationBookingPlatform.API.Common;

public class JwtProvider : IJwtProvider
{
    public string SecretKey { get; }
    public string Issuer { get; }
    public string Audience { get; }

    public JwtProvider(string secretKey, string issuer, string audience)
    {
        SecretKey = secretKey;
        Issuer = issuer;
        Audience = audience;
    }
}