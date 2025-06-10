using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Tests.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.common.DatabaseFactories;

public class InMemoryDbContextFactory : ITestDbContextFactory
{
    public IAppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        return new SqlServerDbContext(options);
    }
}