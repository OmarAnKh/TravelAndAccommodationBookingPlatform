using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Tests.enums;
using TravelAndAccommodationBookingPlatform.Tests.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.common;

public class DbContextFactory : IDbContextFactory
{
    public IAppDbContext Create(DatabaseType type, string? connectionString = null)
    {
        return type switch
        {
            DatabaseType.InMemory => CreateInMemoryContext(connectionString),
            DatabaseType.SqlServer => CreateSqlServerContext(connectionString!),
            _ => throw new ArgumentException($"Unsupported database type: {type}")
        };
    }

    private IAppDbContext CreateInMemoryContext(string? databaseName)
    {
        var dbName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
        var options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        var context = new SqlServerDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private IAppDbContext CreateSqlServerContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new SqlServerDbContext(options);
    }

}