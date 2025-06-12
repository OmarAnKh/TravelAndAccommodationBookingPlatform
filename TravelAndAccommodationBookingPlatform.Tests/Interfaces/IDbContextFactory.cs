using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Interfaces;

public interface IDbContextFactory
{
    IAppDbContext Create(DatabaseType type, string? connectionString = null);
}