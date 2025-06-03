using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly DbContextOptions<SqlServerDbContext> _options;

    public DatabaseFixture()
    {
        _connectionString = $"Server=(localdb)\\mssqllocaldb;Database=TravelPlatformIntegrationTest_{Guid.NewGuid()};Trusted_Connection=True;MultipleActiveResultSets=True";

        _options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
    }

    public SqlServerDbContext CreateContext() => new SqlServerDbContext(_options);

    public async Task InitializeAsync()
    {
        using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }
}